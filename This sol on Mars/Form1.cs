using System;
using System.Drawing;
using System.Drawing.Text;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace This_sol_on_Mars
{
    public partial class Form1 : Form
    {

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        IObservable<Report> pro1 = null;
        // IObservable<List<Report>> pro2 = null;

        IObservable<RootObject> pro3 = null;
        IDisposable pro3subscripton = null;

        Font mars_font;

        public Form1()
        {
            InitializeComponent();
            SynchronizationContextScheduler UIThread = new SynchronizationContextScheduler(SynchronizationContext.Current);
            NewThreadScheduler NewThread = new NewThreadScheduler();

            // Proxy.Connect();
            mars_font = GetFontFromResource(Properties.Resources.newmars, 16.0F);

            pro1 = Observable
                .FromAsync(() => Curiosity.GetLatestData())
                .ObserveOn(UIThread);

            //pro2 = Observable
            //    .FromAsync(() => Curiosity.GetArchiveData());

            pro3 = Curiosity
                .GetArchiveDataY()
                .ToObservable()
                .SubscribeOn(NewThread);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Font = mars_font;

            pro1.Subscribe<Report>(report =>
            {
                label1.Text = $"Sol number: {report.sol}";
                label3.Text = $"Last updated: {report.terrestrial_date} (Earth date)";

                label4.Text = $"{report.max_temp}";
                label5.Text = $"{report.min_temp}";
                label7.Text = $"Weather status: {report.atmo_opacity}";

                label8.Text = $"Sunrise: {report.sunrise}";
                label9.Text = $"Sunset:  {report.sunset}";
                label10.Text = $"Pressure: {report.pressure} [Pa] ({report.pressure_string})";
            }, ex =>
            {
                MessageBox.Show(ex.Message);
            },
            () =>
            {
                pro3subscripton =  pro3.Subscribe(root =>
                {
                    root.results.ForEach(x => Console.WriteLine($"{x.sol}\t{x.terrestrial_date}\t{x.min_temp}\t{x.max_temp}\t{x.pressure}"));
                });
            });
        }

        private Font GetFontFromResource(byte[] fontData, float fontSize)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, fontData.Length);
            AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
            Marshal.FreeCoTaskMem(fontPtr);

            return new Font(fonts.Families[0], fontSize, FontStyle.Regular);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            pro3subscripton.Dispose();
            Curiosity.Closing = true;
        }
    }
}
