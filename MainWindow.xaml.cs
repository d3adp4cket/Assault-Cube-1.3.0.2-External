using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AssaultCubeExternal
{
    public partial class MainWindow : Window
    {
        private Memory      _mem    = new();
        private LocalPlayer? _player;
        private GameWorld?   _world;

        private volatile bool _godMode  = false;
        private volatile bool _infAmmo  = false;
        private volatile bool _fastFire = false;
        private volatile bool _running  = true;

        private static readonly SolidColorBrush Green = new(Color.FromRgb(0x39, 0xFF, 0x14));
        private static readonly SolidColorBrush Red   = new(Color.FromRgb(0xE6, 0x3B, 0x3B));
        private static readonly SolidColorBrush Muted = new(Color.FromRgb(0x44, 0x44, 0x44));

        public MainWindow()
        {
            InitializeComponent();
            this.Title = Guid.NewGuid().ToString(); // Stealth: Randomize window name
            Closed += (_, _) => { _running = false; _mem.Detach(); };
            new Thread(AttachLoop) { IsBackground = true }.Start();
        }

        private void TitleBar_MouseDown(object s, System.Windows.Input.MouseButtonEventArgs e) => DragMove();
        private void CloseClick   (object s, RoutedEventArgs e) => Close();
        private void MinimizeClick(object s, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Feature_Changed(object s, RoutedEventArgs e)
        {
            _godMode  = Toggle_GodMode.IsChecked  == true;
            _infAmmo  = Toggle_Ammo.IsChecked     == true;
            _fastFire = Toggle_FastFire.IsChecked == true;
        }

        // ── ATTACH ───────────────────────────────────────────────────────
        private void AttachLoop()
        {
            while (_running)
            {
                if (!_mem.IsAttached)
                {
                    SetStatus(false, "waiting for ac_client.exe");
                    if (_mem.Attach("ac_client"))
                    {
                        SetStatus(true, "attached");
                        _player = new LocalPlayer(_mem, _mem.BaseAddr);
                        _world  = new GameWorld(_mem, _mem.BaseAddr);

                        new Thread(GameLoop) { IsBackground = true }.Start();
                    }
                }
                Thread.Sleep(1500);
            }
        }

        // ── GAME LOOP ────────────────────────────────────────────────────
        private void GameLoop()
        {
            int tick = 0;
            while (_running)
            {
                if (Process.GetProcessesByName("ac_client").Length == 0)
                {
                    _mem.Detach();
                    Dispatcher.BeginInvoke(() => { SetStatus(false, "process closed"); ClearStats(); });
                    new Thread(AttachLoop) { IsBackground = true }.Start();
                    return;
                }

                try
                {
                    _player!.Refresh();

                    if (!_player.IsValid)
                    {
                        // Stagger diagnostic reads to avoid spamming the logs
                        if (tick % 60 == 0)
                        {
                            var diag = _player.DiagRead();
                            Log($"ptr=0 raw=0x{diag.raw:X8} ok={diag.ok}");
                        }
                        
                        tick++; 
                        Thread.Sleep(15); 
                        continue;
                    }

                    if (_godMode)  _player.GodMode();
                    if (_infAmmo)  _player.InfiniteAmmo();
                    if (_fastFire) _player.FastFire();

                    var pPos = new Vec3 { X = _player.PosX, Y = _player.PosY, Z = _player.PosZ };

                    // Cache values before dispatching to the UI thread
                    var stats = new
                    {
                        Health = _player.Health,
                        Armor = _player.Armor,
                        Name = _player.Name,
                        Pos = pPos,
                        PlayerCount = _world!.PlayerCount(),
                        Ammo = new
                        {
                            AR = _player.AmmoAR,
                            SMG = _player.AmmoSMG,
                            Sniper = _player.AmmoSniper,
                            Shotgun = _player.AmmoShotgun,
                            Pistol = _player.AmmoPistol,
                            Grenade = _player.AmmoGrenade
                        }
                    };

                    Dispatcher.BeginInvoke(() =>
                    {
                        HealthVal.Text      = stats.Health.ToString();
                        ArmorVal.Text       = stats.Armor.ToString();
                        PlayerNameText.Text = stats.Name;

                        AmmoAR.Text      = stats.Ammo.AR.ToString();
                        AmmoSMG.Text     = stats.Ammo.SMG.ToString();
                        AmmoSniper.Text  = stats.Ammo.Sniper.ToString();
                        AmmoShotgun.Text = stats.Ammo.Shotgun.ToString();
                        AmmoPistol.Text  = stats.Ammo.Pistol.ToString();
                        AmmoGrenade.Text = stats.Ammo.Grenade.ToString();

                        PosText.Text     = $"X {stats.Pos.X:F0}   Y {stats.Pos.Y:F0}   Z {stats.Pos.Z:F0}";
                        FooterLog.Text   = $"hp:{stats.Health}  arm:{stats.Armor}  pc:{stats.PlayerCount}";
                        
                        HealthVal.Foreground = stats.Health < 30 ? Red : Green;
                    }, DispatcherPriority.Background);
                }
                catch (Exception ex) { Log($"err: {ex.Message}"); }

                tick++;
                Thread.Sleep(15);
            }
        }

        private void ClearStats()
        {
            HealthVal.Text = ArmorVal.Text = "—";
            AmmoAR.Text = AmmoSMG.Text = AmmoSniper.Text =
            AmmoPistol.Text = AmmoShotgun.Text = AmmoGrenade.Text = "—";
            PosText.Text = ""; PlayerNameText.Text = "";
            FooterLog.Text = "disconnected";
        }

        private void SetStatus(bool ok, string msg) => Dispatcher.BeginInvoke(() =>
        {
            StatusDot.Fill        = ok ? Green : Muted;
            StatusText.Text       = msg;
            StatusText.Foreground = ok ? Green : Muted;
        });

        private void Log(string msg) => Dispatcher.BeginInvoke(() => FooterLog.Text = msg);
    }
}
