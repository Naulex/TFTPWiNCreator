using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Drawing;

namespace TFTPWiNCreator
{
    public partial class MainForm : Form
    {
        Process TFTP = new Process();

        public MainForm()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            InfoBox.Text += "TFTPWiNCreator - программа, позволяющая автоматизировать процесс создания загрузочных PXE-серверов.\r\n\r\nПрограмма извлекает из оригинального ISO образа ОС предзагрузочную среду, модифицирует её и добавляет изменённый загрузчик, а затем создаёт tftp сервер для подключения.\r\n\r\nПрограмма использует функционал следующих приложений:\r\n-7zip\r\n-TFTPd64\r\n-Bootice64\r\n\r\nv.1.0: добавлена поддержка загрузчика Syslinux.\r\n\r\nv2.0: Изменения и улучшения.\r\n\r\n(c)Naulex, 2023\r\n073797@gmail.com\r\n\r\n\r\n";
            InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Запуск выполнен успешно.\r\n";
            SoftExtractorDialog.SelectedPath = Environment.CurrentDirectory;
        }
        private void openNetSettings_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "ncpa.cpl";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Открытие настроек сети...\r\n";
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }

        private void openFirewallSettings_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "control.exe";
            process.StartInfo.Arguments = "firewall.cpl";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Открытие настроек брандмауэра...\r\n";
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }

        private void BeginBTN_Click(object sender, EventArgs e)
        {

            if (IpBox.Text.Length == 0 || OpenISODialog.FileName.Length == 0)
            {
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Ошибка! Не выбран образ системы или не указан IP!\r\n";
                return;
            }
            IpBox.Enabled = false;
            LoaderChooseGroup.Enabled = false;
            WinBootMGRGroup.Enabled = false;
            DelResFolder.Enabled = false;

            if (Directory.Exists("Resources"))
            {
                MessageBox.Show("Обнаружена существующая папка \"Resources\". В целях исключения ошибок в работе программы, эта папка будет удалена сразу после закрытия этого окна. Если там имеются важные файлы, скопируйте их, и только потом закрывайте это окно.", "Обнаружена существующая папка | TFTPWiNCreator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Directory.Delete("Resources", true);
            }
            InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Создание папки Resources...\r\n";
            Directory.CreateDirectory("Resources");
            InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла tftpd64...\r\n";
            File.WriteAllBytes(@"Resources\tftpd64.exe", Properties.Resources.tftpd64);


            if (ISOChoosedTFTP.Checked == true)
            {
                if (WinBootManager.Checked == true)
                {
                    int BiosUefi = 0;
                    if (BIOSbutton.Checked == true)
                    { BiosUefi = 1; }
                    if (UEFIbutton.Checked == true)
                    { BiosUefi = 2; }
                    BeginTuningWindows(IpBox.Text, BiosUefi, OpenISODialog.FileName, false);
                }
                if (Syslinux.Checked == true)
                {
                    BeginTuningSyslinux(IpBox.Text, OpenISODialog.FileNames, 3);
                }
            }
            if (FolderChoosedTFTP.Checked == true)
            {
                if (WinBootManager.Checked == true)
                {
                    int BiosUefi = 0;
                    if (BIOSbutton.Checked == true)
                    { BiosUefi = 1; }
                    if (UEFIbutton.Checked == true)
                    { BiosUefi = 2; }
                    BeginTuningWindows(IpBox.Text, BiosUefi, OpenFolderTFTP.SelectedPath, true);
                }
            }

        }

        void BeginTuningSyslinux(string IP, string[] OSPaths, int BiosUefi)
        {
            try
            {
                if (OSPaths.Length == 0)
                {
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Ошибка! Не выбран ни один образ системы!\r\n";
                    return;
                }
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Создание папки pxelinux.cfg...\r\n";
                Directory.CreateDirectory(@"Resources\pxelinux.cfg");

                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Генерация файла default...\r\n";
                using (StreamWriter sw = new StreamWriter(@"Resources\pxelinux.cfg\default", false))
                {
                    sw.WriteLine("default vesamenu.c32");
                    sw.WriteLine("MENU BACKGROUND pxelinux.cfg/1.png");
                    sw.WriteLine("ALLOWOPTIONS 0");
                    sw.WriteLine("PROMPT 0");
                    sw.WriteLine("MENU TITLE Auto-generated by TFTPWiNCreator");
                    sw.WriteLine("MENU RESOLUTION 800 600");
                    sw.WriteLine("MENU COLOR border 30;40 #000000ff #000000ff none");
                    sw.WriteLine("MENU COLOR title 1;36;40 #ffffffff #ff3d54f3 std");
                    sw.WriteLine("MENU COLOR unsel 37;40 #ffffffff #00ffffff none");
                    sw.WriteLine("MENU COLOR hotkey 1;37;40 #ffffffff #00000000 std");
                    sw.WriteLine("MENU COLOR sel 0;37;40 #ff3d54f3 #00010003 none");
                    sw.WriteLine("MENU COLOR scrollbar 30;40 #ff000000 #ff000000 std");
                    sw.WriteLine("MENU COLOR help 30;40 #ff427c2b #ff010003 none");
                    sw.WriteLine("MENU COLOR timeout_msg 37;40 #ff427c2b #00010003 none");
                    sw.WriteLine("MENU COLOR timeout 1;37;40 #ff427c2b #00010003 none");
                    sw.WriteLine("MENU WIDTH 40");
                    sw.WriteLine("MENU MARGIN 1");
                    sw.WriteLine("MENU ENDROW -1");
                    sw.WriteLine("MENU HELPMSGROW 32");
                    sw.WriteLine("MENU HELPMSGENDROW -1");
                    sw.WriteLine("MENU TIMEOUTROW 12");
                    sw.WriteLine("MENU HSHIFT 0");
                    sw.WriteLine("MENU VSHIFT 0");
                    sw.WriteLine("TIMEOUT 300");
                    sw.WriteLine("");

                    foreach (string path in OSPaths)
                    {
                        var fileName = Path.GetFileName(path);
                        fileName = fileName.Replace("/", "");
                        fileName = fileName.Replace(@"\", "");
                        fileName = fileName.Replace(":", "");
                        fileName = fileName.Replace("*", "");
                        fileName = fileName.Replace("?", "");
                        fileName = fileName.Replace("\"", "");
                        fileName = fileName.Replace("<", "");
                        fileName = fileName.Replace(">", "");
                        fileName = fileName.Replace("|", "");
                        fileName = fileName.Replace("-", "");
                        fileName = fileName.Replace(" ", "");

                        sw.WriteLine("label " + fileName);
                        sw.WriteLine("  menu " + fileName);
                        sw.WriteLine("  kernel memdisk iso");
                        sw.WriteLine("  initrd " + fileName);
                        sw.WriteLine("  append iso raw");
                        sw.WriteLine("");

                        InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Перемещение образа " + Path.GetFileName(path) + " из " + path + " в Resources\\" + "...\r\n";
                        File.Copy(path, @"Resources\" + fileName);
                    }

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла 1.png...\r\n";
                    Properties.Resources._1.Save(@"Resources\pxelinux.cfg\1.png", System.Drawing.Imaging.ImageFormat.Png);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла chain.c32...\r\n";
                    File.WriteAllBytes(@"Resources\chain.c32", Properties.Resources.chain);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла ldlinux.c32...\r\n";
                    File.WriteAllBytes(@"Resources\ldlinux.c32", Properties.Resources.ldlinux);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла libcom32.c32...\r\n";
                    File.WriteAllBytes(@"Resources\libcom32.c32", Properties.Resources.libcom32);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла libcom32.elf...\r\n";
                    File.WriteAllBytes(@"Resources\libcom32.elf", Properties.Resources.libcom321);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла libutil.c32...\r\n";
                    File.WriteAllBytes(@"Resources\libutil.c32", Properties.Resources.libutil);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла linux.c32...\r\n";
                    File.WriteAllBytes(@"Resources\linux.c32", Properties.Resources.linux);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла lpxelinux.0...\r\n";
                    File.WriteAllBytes(@"Resources\lpxelinux.0", Properties.Resources.lpxelinux);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла memdisk...\r\n";
                    File.WriteAllBytes(@"Resources\memdisk", Properties.Resources.memdisk);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла chain.c32...\r\n";
                    File.WriteAllBytes(@"Resources\chain.c32", Properties.Resources.menu);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла pxelinux.0...\r\n";
                    File.WriteAllBytes(@"Resources\pxelinux.0", Properties.Resources.pxelinux);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла vesamenu.c32...\r\n";
                    File.WriteAllBytes(@"Resources\vesamenu.c32", Properties.Resources.vesamenu);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Генерация файла tftpd32.ini...\r\n";
                    CreateSettings(IP, BiosUefi);

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Вызов приложения tftpd64.exe...\r\n";
                    TFTP = Process.Start(@"Resources\tftpd64.exe");

                    InfoBox.Text += "========================================\r\n";
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Сервер создан успешно! Управление передано программе tftpd64.exe. Загружайте ОС по PXE.\r\n";
                    InfoBox.Text += "========================================\r\n";
                    LoaderChooseGroup.Enabled = true;
                    IpBox.Enabled = true;
                    WinBootMGRGroup.Enabled = true;
                    DelResFolder.Enabled = true;
                }
            }
            catch
            { MessageBox.Show("Непредвиденная ошибка!", "Ошибка | TFTPWiNCreator", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        async void BeginTuningWindows(string IP, int BiosUefi, string OSPath, bool IsoOrFolder)
        {
            try
            {
                if (IP.Length == 0 || OSPath.Length == 0)
                {
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Ошибка! Не выбран образ системы или не указан IP!\r\n";
                    return;
                }
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Начало работы...\r\n";
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла BCD_BIOS...\r\n";
                File.WriteAllBytes(@"Resources\BCD_BIOS", Properties.Resources.BCD_BIOS);
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла bcd_uefi...\r\n";
                File.WriteAllBytes(@"Resources\bcd_uefi", Properties.Resources.bcd_uefi);
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла BOOTICEx64...\r\n";
                File.WriteAllBytes(@"Resources\BOOTICEx64.exe", Properties.Resources.BOOTICEx64);
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла init.cmd...\r\n";
                File.WriteAllBytes(@"Resources\init.cmd", Properties.Resources.init);

                if (IsoOrFolder == false)
                {
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла 7z.dll...\r\n";
                    File.WriteAllBytes(@"Resources\7z.dll", Properties.Resources._7z1);
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла 7z.exe...\r\n";
                    File.WriteAllBytes(@"Resources\7z.exe", Properties.Resources._7z);
                }
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Генерация файла winpeshl.ini...\r\n";
                using (StreamWriter sw = new StreamWriter(@"Resources\winpeshl.ini", false))
                {
                    sw.WriteLine("[LaunchApps]");
                    sw.Write("init.cmd " + (IP.Replace(" ", "")).Replace(",", "."));
                }
                if (IsoOrFolder == false)
                {
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Вызов 7z.exe, извлечение образа " + OSPath + "...\r\n";
                    Process extract = new Process();
                    extract.StartInfo.FileName = @"Resources\7z.exe";
                    extract.StartInfo.Arguments = "x -o\"" + Environment.CurrentDirectory + "\\Resources\\WinISO\" " + "\"" + OSPath + "\"";
                    extract.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    extract.Start();
                    extract.WaitForExit();
                    extract.Dispose();
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Успешно!\r\n";
                }
                if (IsoOrFolder == true)
                {
                    Directory.CreateDirectory(@"Resources\WinISO");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файлов из образа в папку назначения...\r\n";
                    await Task.Run(() =>
                        {
                            CopyFolder(OSPath, Environment.CurrentDirectory + @"\Resources\WinISO");
                        });
                }

                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Создание папки mnt...\r\n";
                Directory.CreateDirectory(@"Resources\mnt");
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Создание папки boot...\r\n";
                Directory.CreateDirectory(@"Resources\boot");

                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Вызов dism.exe, монтирование образа boot.wim...\r\n";
                Process DismMount = new Process();
                DismMount.StartInfo.FileName = @"dism.exe";
                DismMount.StartInfo.Arguments = " /mount-wim /wimfile:\"" + Environment.CurrentDirectory + "\\Resources\\WinISO\\sources\\boot.wim\" /index:2 /mountdir:\"" + Environment.CurrentDirectory + "\\Resources\\mnt\"";
                DismMount.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                DismMount.Start();
                DismMount.WaitForExit();
                DismMount.Dispose();
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Успешно!\r\n";


                if (BiosUefi == 1)
                {
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла pxeboot.n12...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\mnt\Windows\Boot\PXE\pxeboot.n12", Environment.CurrentDirectory + @"\Resources\pxeboot.n12");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла bootmgr.exe...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\mnt\Windows\Boot\PXE\bootmgr.exe", Environment.CurrentDirectory + @"\Resources\bootmgr.exe");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла boot.sdi...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\mnt\Windows\Boot\DVD\PCAT\boot.sdi", Environment.CurrentDirectory + @"\Resources\boot\boot.sdi");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла memtest.exe...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\mnt\Windows\Boot\PCAT\memtest.exe", Environment.CurrentDirectory + @"\Resources\boot\memtest.exe");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование папки ru-RU...\r\n";
                    CopyDir(Environment.CurrentDirectory + @"\Resources\mnt\Windows\Boot\PXE\ru-RU", Environment.CurrentDirectory + @"\Resources\boot\ru-RU");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование папки Fonts...\r\n";
                    CopyDir(Environment.CurrentDirectory + @"\Resources\mnt\Windows\Boot\Fonts", Environment.CurrentDirectory + @"\Resources\boot\Fonts");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла BCD...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\BCD_BIOS", Environment.CurrentDirectory + @"\Resources\boot\BCD");

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла init.cmd...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\init.cmd", Environment.CurrentDirectory + @"\Resources\mnt\Windows\System32\init.cmd");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла winpeshl.ini...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\winpeshl.ini", Environment.CurrentDirectory + @"\Resources\mnt\Windows\System32\winpeshl.ini");

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Генерация файла tftpd32.ini...\r\n";
                    CreateSettings(IP, BiosUefi);

                }


                if (BiosUefi == 2)
                {
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование папки boot...\r\n";
                    CopyDir(Environment.CurrentDirectory + @"\Resources\WinISO\efi\microsoft\boot", Environment.CurrentDirectory + @"\Resources\boot");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла bootx64.efi...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\WinISO\efi\boot\bootx64.efi", Environment.CurrentDirectory + @"\Resources\bootx64.efi");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла boot.sdi...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\WinISO\boot\boot.sdi", Environment.CurrentDirectory + @"\Resources\boot\boot.sdi");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Удаление стандартного файла bcd...\r\n";
                    File.Delete(Environment.CurrentDirectory + @"\Resources\boot\bcd");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла bcd...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\bcd_uefi", Environment.CurrentDirectory + @"\Resources\boot\bcd");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла init.cmd...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\init.cmd", Environment.CurrentDirectory + @"\Resources\mnt\Windows\System32\init.cmd");
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Копирование файла winpeshl.ini...\r\n";
                    File.Copy(Environment.CurrentDirectory + @"\Resources\winpeshl.ini", Environment.CurrentDirectory + @"\Resources\mnt\Windows\System32\winpeshl.ini");

                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Генерация файла tftpd32.ini...\r\n";
                    CreateSettings(IP, BiosUefi);

                }


                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Вызов dism.exe, размонтирование образа boot.wim...\r\n";
                Process DismUnMount = new Process();
                DismUnMount.StartInfo.FileName = @"dism.exe";
                DismUnMount.StartInfo.Arguments = " /unmount-wim /mountdir:\"" + Environment.CurrentDirectory + "\\Resources\\mnt\" /commit";
                DismUnMount.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                DismUnMount.Start();
                DismUnMount.WaitForExit();
                DismUnMount.Dispose();
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Успешно!\r\n";

                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Перемещение папки Sources...\r\n";
                Directory.Move(Environment.CurrentDirectory + @"\Resources\WinISO\Sources", Environment.CurrentDirectory + @"\Resources\sources");
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Успешно!\r\n";


                if (AddEI.Checked == true)
                {
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Извлечение файла ei.cfg...\r\n";
                    File.WriteAllBytes(@"Resources\sources\ei.cfg", Properties.Resources.ei);
                }



                OpenAccessBTN.Enabled = true;
                Item6.Text = "6) Откройте общий доступ к папке \"sources\"";

                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Вызов приложения tftpd64.exe...\r\n";
                TFTP = Process.Start(@"Resources\tftpd64.exe");


                InfoBox.Text += "========================================\r\n";
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Сервер создан успешно! Управление передано программе tftpd64.exe. Откройте общий доступ для папки sources и загружайте ОС по PXE.\r\n";
                InfoBox.Text += "========================================\r\n";
                LoaderChooseGroup.Enabled = true;
                IpBox.Enabled = true;
                WinBootMGRGroup.Enabled = true;
                DelResFolder.Enabled = true;
            }
            catch
            { MessageBox.Show("Непредвиденная ошибка!", "Ошибка | TFTPWiNCreator", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        void CreateSettings(string IP, int BiosUefi)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(@"Resources\tftpd32.ini", false))
                {
                    sw.WriteLine("[DHCP]");
                    sw.WriteLine("IP_Pool=" + (IP.Replace(" ", "")).Replace(",", "."));
                    sw.WriteLine("PoolSize=10");
                    if (BiosUefi == 1)
                    { sw.WriteLine("BootFile=pxeboot.n12"); }
                    if (BiosUefi == 2)
                    { sw.WriteLine("BootFile=bootx64.efi"); }
                    if (BiosUefi == 3)
                    { sw.WriteLine("BootFile=pxelinux.0"); }
                    sw.WriteLine("Mask=255.0.0.0");
                    sw.WriteLine("Gateway=" + (IP.Replace(" ", "")).Replace(",", "."));
                    sw.WriteLine("Lease (minutes)=2880");
                    sw.WriteLine("[TFTPD32]");
                    sw.WriteLine(@"BaseDirectory=" + Environment.CurrentDirectory + @"\Resources");
                    sw.WriteLine("TftpPort =69");
                    if (BiosUefi == 2)
                    { sw.WriteLine("WinSize=4096"); }
                    sw.WriteLine("Negociate=1");
                    sw.WriteLine("ShowProgressBar=1");
                    sw.WriteLine("Timeout=3");
                    sw.WriteLine("MaxRetransmit=6");
                    sw.WriteLine("SecurityLevel=1");
                    sw.WriteLine("UnixStrings=1");
                    sw.WriteLine("VirtualRoot=1");
                    sw.WriteLine("LocalIP=" + (IP.Replace(" ", "")).Replace(",", "."));
                    sw.WriteLine("Services=5");
                    sw.WriteLine("PersistantLeases=1");
                    sw.WriteLine("DHCP Ping=1");
                    sw.WriteLine("DHCP Double Answer=1");
                    sw.WriteLine("DHCP LocalIP=" + (IP.Replace(" ", "")).Replace(",", "."));
                    sw.WriteLine("Max Simultaneous Transfers=100");
                    sw.WriteLine("Console Password=tftpd32");
                    sw.Write("Keep transfer Gui=5");
                }
            }
            catch
            { MessageBox.Show("Непредвиденная ошибка!", "Ошибка | TFTPWiNCreator", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        void CopyDir(string FromDir, string ToDir)
        {
            try
            {
                Directory.CreateDirectory(ToDir);
                foreach (string s1 in Directory.GetFiles(FromDir))
                {
                    string s2 = ToDir + "\\" + Path.GetFileName(s1);
                    File.Copy(s1, s2);
                }
                foreach (string s in Directory.GetDirectories(FromDir))
                {
                    CopyDir(s, ToDir + "\\" + Path.GetFileName(s));
                }
            }
            catch { }
        }
        private void ChooseFolderTFTPButton_Click(object sender, EventArgs e)
        {

            if (OpenFolderTFTP.ShowDialog() == DialogResult.Cancel)
                return;

            if (OpenFolderTFTP.SelectedPath.Length == 0)
            {
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Ошибка! Не указана целевая папка!\r\n";
                return;
            }
            InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Выбрана папка: " + OpenFolderTFTP.SelectedPath + "\r\n";
        }
        private void ChooseISOBTN_Click(object sender, EventArgs e)
        {
            if (OpenISODialog.ShowDialog() == DialogResult.Cancel)
                return;
            if (OpenISODialog.Multiselect == false)
            {
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Выбран ISO образ: " + OpenISODialog.FileName + "\r\n";
            }
            if (OpenISODialog.Multiselect == true)
            {
                foreach (string path in OpenISODialog.FileNames)
                {
                    InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Выбран ISO образ [" + Convert.ToInt32(Array.IndexOf(OpenISODialog.FileNames, path) + 1) + "/" + OpenISODialog.FileNames.Length + "]: " + path + "\r\n";
                }
            }
        }

        private void OpenAccessBTN_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Environment.CurrentDirectory + @"\Resources\sources");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                TFTP.Kill();
            }
            catch
            { }
        }


        private void BIOSbutton_CheckedChanged(object sender, EventArgs e)
        {
            if (BIOSbutton.Checked == true)
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Выбран интерфейс: BIOS...\r\n";
        }

        private void UEFIbutton_CheckedChanged(object sender, EventArgs e)
        {
            if (UEFIbutton.Checked == true)
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Выбран интерфейс: UEFI...\r\n";
        }

        private void InfoBox_TextChanged(object sender, EventArgs e)
        {
            InfoBox.SelectionStart = InfoBox.Text.Length;
            InfoBox.ScrollToCaret();
            InfoBox.Refresh();
        }

        private void AddEI_CheckedChanged(object sender, EventArgs e)
        {
            if (AddEI.Checked == true)
            {
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Файл ei.cfg будет добавлен...\r\n";
            }
            else
            {
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Файл ei.cfg не будет добавлен...\r\n";
            }
        }

        private void WinBootManager_CheckedChanged(object sender, EventArgs e)
        {
            if (WinBootManager.Checked == true)
            {
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Выбран загрузчик: Windows Boot Manager...\r\n";
                OpenISODialog.Multiselect = false;
                ChooseISOBTN.Text = "Выбрать образ ОС";
                OpenISODialog.Title = "Выберите ISO образ ОС";
                WinBootMGRGroup.Enabled = true;
            }
        }

        private void Syslinux_CheckedChanged(object sender, EventArgs e)
        {
            if (Syslinux.Checked == true)
            {
                InfoBox.Text += Convert.ToString(DateTime.Now.ToShortTimeString()) + ": Выбран загрузчик: Syslinux. Доступен множественный выбор ISO образов...\r\n";
                ChooseISOBTN.Text = "Выбрать образы ОС";
                OpenISODialog.Title = "Выберите один или несколько ISO образов ОС";
                OpenISODialog.Multiselect = true;
                WinBootMGRGroup.Enabled = false;
            }
        }

        private void DelResFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("Resources"))
            {
                DialogResult DeleteOrNot = MessageBox.Show("Вы действительно хотите удалить эту папку?", "Удаление папки Resources | TFTPWiNCreator", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (DeleteOrNot == DialogResult.Yes)
                {
                    Directory.Delete("Resources", true);
                    MessageBox.Show("Папка удалена.", "Успех | TFTPWiNCreator", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            { MessageBox.Show("Папка не обнаружена.", "Папка не обнаружена | TFTPWiNCreator", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void CopyFolder(string sourceFolder, string destFolder)
        {

            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);


            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(destFolder, Path.GetFileName(file)));

            }

            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string folder in folders)
            {
                CopyFolder(folder, Path.Combine(destFolder, Path.GetFileName(folder)));
            }

        }

        private void ISOChoosedTFTP_CheckedChanged(object sender, EventArgs e)
        {
            if (ISOChoosedTFTP.Checked)
            { ChooseISOBTN.Enabled = true; }
            else
            { ChooseISOBTN.Enabled = false; }
        }

        private void FolderChoosedTFTP_CheckedChanged(object sender, EventArgs e)
        {
            if (FolderChoosedTFTP.Checked)
            {
                ChooseFolderTFTPButton.Enabled = true;
                Syslinux.Enabled = false;
                Syslinux.Checked = false;
                OpenISODialog.Multiselect = false;
                WinBootMGRGroup.Enabled = true;
                WinBootManager.Checked = true;
            }
            else
            {
                ChooseFolderTFTPButton.Enabled = false;
                Syslinux.Enabled = true;
            }
        }
    }
}
