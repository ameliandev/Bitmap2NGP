using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGP_Bitmap_2_File.Core.Clases;

namespace NGP_Bitmap_2_File
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayDefault();
                return;
            }
            if (args[0].Equals("--help"))
            {
                DisplayHelp();
                return;
            }
            //"F:\TRABAJO\DESARROLLO\PROYECTOS\ngpc-bastards\assets\Prueba de Interfaz\portrait.gif" "221,221,221" "85,85,68" "68,68,68" "PS" "-ipd"
            CLParams nbcParams = new CLParams(args);

            if (!nbcParams.IsValidParams)
            {
                DisplayErrors(nbcParams.ErrorMessage);
                Console.Read();
                return;
            }

            NGPImage ngpImage = new NGPImage(nbcParams);

            if (ngpImage.HasHexRows())
            {
                new NGPFile(ngpImage, nbcParams);
            }
        }

        private static void DisplayDefault()
        {
            Console.WriteLine("bitmap2ngp: You must specify one of required params.");
            Console.WriteLine("Try 'bitmap2ngp --help' for more information");
        }

        private static void DisplayErrors(List<string> errors)
        {
            Console.WriteLine("** ERRORS DETECTED **");
            Console.WriteLine("-------------------------------------------------------");
            foreach (var error in errors)
            {
                Console.WriteLine(string.Format("\t- {0}", error));
            }
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Try 'bitmap2ngp --help' for more information");
        }

        private static void DisplayHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Bitmap to Neo Geo Pocket convert a tiled image to compatible Neo Geo Pocket .c file.");
            Console.WriteLine(@"Usage: bitmap2ngp.exe [OPTIONS] FILEPATH PLANE RGBCOLOR1 [RGBCOLOR2] [RGBCOLOR3]");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine(@"   bitmap2ngp.exe ./tiledImage.png P2 68,68,68 126,13,31 255,0,255 -i");
            Console.WriteLine(@"   bitmap2ngp.exe ./tiledImage.png PS 68,68,68 100,145,255");
            Console.WriteLine(@"   bitmap2ngp.exe ./tiledImage.png P1 68,68,68 -ipd");
            Console.WriteLine();
            Console.WriteLine("Valid image formats:");
            Console.WriteLine(" PNG, PCX, GIF");
            Console.WriteLine();
            Console.WriteLine("Generate code file:");
            Console.WriteLine();
            Console.WriteLine(" [] = Optional parameter");
            Console.WriteLine();
            Console.WriteLine("\t FILEPATH \t give de complete path of the tiled image.");
            Console.WriteLine("\t PLANE    \t sets the destination plane of the console. Possible options");
            Console.WriteLine("\t          \t are PS = SPRITE_PLANE, P2 = SCR_2_PLANE, P1 = SCR_1_PLANE.");
            Console.WriteLine("\t RGBCOLOR1\t a minimun required color. Sets the R G B color without");
            Console.WriteLine("\t          \t Alpha channel. Format => R,G,B. Example 128,2,88.");
            Console.WriteLine("\t [RGBCOLOR2]\t 2nd RGB Color");
            Console.WriteLine("\t [RGBCOLOR3]\t 3rd RGB Color");
            Console.WriteLine();
            Console.WriteLine(" Options");
            Console.WriteLine();
            Console.WriteLine("\t -I       \t generates a piece of code to install the generated");
            Console.WriteLine("\t          \t tile in NGP Memory with InstallTileSetAt method.");
            Console.WriteLine("\t -P       \t generates a piece of code to install the selected");
            Console.WriteLine("\t          \t pallete for the seletec plane. Method InstallTilePallete.");
            Console.WriteLine("\t -D       \t generates a piece of code ready to call it and.");
            Console.WriteLine("\t          \t display the tile in the selected plane");
        }
    }
}
