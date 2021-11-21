using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NGP_Bitmap_2_File.Core.Clases
{
    class NGPFile
    {
        public NGPFile(NGPImage PreProcessImage, CLParams commandLineaParams)
        {
            string fileName = string.Format(@"{0}\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "b2ngp.c");

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                if (commandLineaParams.GenerarPaleta)
                {
                    WritePalleteStruct(commandLineaParams, streamWriter);
                    WritePalleteAdapter(commandLineaParams, streamWriter);
                }

                WriteArray(PreProcessImage, streamWriter);

                if (commandLineaParams.GenerarInstall)
                {
                    WriteInstall(commandLineaParams, streamWriter);
                }

                if (commandLineaParams.GenerarPaleta)
                {
                    WritePallete(commandLineaParams, streamWriter);
                }

                if (commandLineaParams.GenerarInstallOnScreen)
                {
                    WriteInstallOnScreen(commandLineaParams, streamWriter);
                }
            }
        }

        /// <summary>
        /// Escribe en el fichero el array de pixeles
        /// </summary>
        /// <param name="PreProcessImage"></param>
        /// <param name="fs"></param>
        private void WriteArray(NGPImage PreProcessImage, StreamWriter streamWriter)
        {
            int rowNumber = 0;

            streamWriter.WriteLine(string.Format("const unsigned short ngTile[{0}][8] =", PreProcessImage.HexRows.Count / 8));
            streamWriter.WriteLine("{");
            streamWriter.WriteLine();

            for (int i = 0; i < PreProcessImage.HexRows.Count; i++)
            {
                if (Utils.IsMultiploDe(rowNumber, 8) && rowNumber != 0)
                {
                    streamWriter.Write("},");
                    streamWriter.WriteLine();
                    rowNumber = 0;
                }

                if (rowNumber == 0)
                {
                    streamWriter.Write("\t");
                    streamWriter.Write("{");
                }

                //byte[] hex = new UTF8Encoding(true).GetBytes(string.Format(" {0}", PreProcessImage.HexRows[i]));

                streamWriter.Write(string.Format(" {0}{1}", PreProcessImage.HexRows[i], (rowNumber == 7) ? " " : ""));

                if (rowNumber != 7)
                {
                    streamWriter.Write(",");
                }

                rowNumber++;

                if (i == (PreProcessImage.HexRows.Count - 1))
                {
                    streamWriter.WriteLine("}");
                }
            }

            streamWriter.Write("");
            streamWriter.WriteLine("};");
            streamWriter.WriteLine();
        }

        /// <summary>
        /// Escribe en el fichero el array de pixeles
        /// </summary>
        /// <param name="PreProcessImage"></param>
        /// <param name="streamWriter"></param>
        private void WriteInstall(CLParams commandLineParams, StreamWriter streamWriter)
        {
            //byte[] title = new UTF8Encoding(true).GetBytes("void InstallTile(unsigned short memoryPosition)");

            streamWriter.Write("void InstallTile(unsigned short memoryPosition)");
            streamWriter.WriteLine();
            streamWriter.Write("{");
            streamWriter.WriteLine();

            //WritePallete(commandLineParams, streamWriter);            

            streamWriter.Write("\t");
            streamWriter.Write("InstallTileSetAt(&ngTile, sizeof(ngTile)/2, memoryPosition);");
            streamWriter.WriteLine();

            streamWriter.Write("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();
        }

        /// <summary>
        /// Escribe en el fichero la instalación de la paleta
        /// </summary>
        /// <param name="PreProcessImage"></param>
        /// <param name="streamWriter"></param>
        private void WritePallete(CLParams commandLineParams, StreamWriter streamWriter)
        {
            streamWriter.Write("void InstallTilePallete()");
            //streamWriter.Write("void InstallTilePallete(unsigned short Plane)");
            streamWriter.WriteLine();
            streamWriter.Write("{");
            streamWriter.WriteLine();

            //streamWriter.Write("\t");
            //streamWriter.Write("GamePal pallete;");
            //streamWriter.WriteLine();
            //streamWriter.WriteLine();

            streamWriter.Write("\t");
            //streamWriter.Write("pallete.PLANE = Plane;");
            streamWriter.Write(string.Format("pallete.PLANE = {0};", commandLineParams.Plane));
            streamWriter.WriteLine();

            streamWriter.Write("\t");
            //streamWriter.Write("pallete.PAL = getPalNumberFromPlane(Plane);");
            streamWriter.Write(string.Format("pallete.PAL = getPalNumberFromPlane({0});", commandLineParams.Plane));
            streamWriter.WriteLine();

            streamWriter.Write("\t");
            streamWriter.Write(string.Format("pallete.Color1 = RGB2NGPColor({0},{1},{2});", commandLineParams.Color1.R, commandLineParams.Color1.G, commandLineParams.Color1.B));
            streamWriter.WriteLine();

            streamWriter.Write("\t");
            streamWriter.Write(string.Format("pallete.Color2 = RGB2NGPColor({0},{1},{2});", commandLineParams.Color2.R, commandLineParams.Color2.G, commandLineParams.Color2.B));
            streamWriter.WriteLine();

            streamWriter.Write("\t");
            streamWriter.Write(string.Format("pallete.Color3 = RGB2NGPColor({0},{1},{2});", commandLineParams.Color3.R, commandLineParams.Color3.G, commandLineParams.Color3.B));
            streamWriter.WriteLine();
            streamWriter.WriteLine();

            streamWriter.Write("\t");
            streamWriter.Write(string.Format("InstallPallete(pallete);", commandLineParams.Plane));
            streamWriter.WriteLine();

            streamWriter.Write("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();
        }

        private void WritePalleteAdapter(CLParams commandLineaParams, StreamWriter streamWriter)
        {
            streamWriter.WriteLine("/*");
            streamWriter.WriteLine(" * Converts a 256 RGB color to adapted Neo Geo Pocket Color (16 colors).");
            streamWriter.WriteLine(" * @param unsigned short R - Red.");
            streamWriter.WriteLine(" * @param unsigned short G - Green.");
            streamWriter.WriteLine(" * @param unsigned short B - Blue.");
            streamWriter.WriteLine(" * @return an adapted rgb color to Neo Geo Pocket as unsigned short.");
            streamWriter.WriteLine(" */");
            streamWriter.WriteLine("unsigned short RGB2NGPColor(unsigned short R, unsigned short G, unsigned short B)");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\t //RGB method its in ngpc.h file in the framework.");
            streamWriter.WriteLine("\t return RGB(R / 16, G / 16, B / 16);");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
        }

        private void WritePalleteStruct(CLParams commandLineaParams, StreamWriter streamWriter)
        {
            //streamWriter.WriteLine("/*");
            streamWriter.WriteLine("typedef struct gamepal");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tunsigned char PLANE;");
            streamWriter.WriteLine("\tunsigned char PAL;");
            streamWriter.WriteLine("\tunsigned char Color1;");
            streamWriter.WriteLine("\tunsigned char Color2;");
            streamWriter.WriteLine("\tunsigned char Color3;");
            streamWriter.WriteLine("} GamePal;");
            //streamWriter.WriteLine("*/");
            streamWriter.WriteLine();
            streamWriter.Write("GamePal pallete;");
            streamWriter.WriteLine();
            streamWriter.WriteLine();
        }

        private void WriteInstallOnScreen(CLParams commandLineParams, StreamWriter streamWriter)
        {
            short memoryPossition = -1;
            int spriteNo = 0;

            streamWriter.WriteLine();
            streamWriter.WriteLine("void DisplayTile(unsigned short memoryPoss, unsigned short xPosition, unsigned short yPosition)");
            streamWriter.WriteLine("{");

            for (int x = 0; x < commandLineParams.ImagenNGP.Ancho; x++)
            {
                for (int y = 0; y < commandLineParams.ImagenNGP.Alto; y++)
                {
                    memoryPossition++;
                    streamWriter.WriteLine(string.Format("\tSetSprite({0}, memoryPoss + {1}, {2}, xPosition + {3}, yPosition + {4}, {5});", spriteNo, memoryPossition, 0, x * 8 , y * 8, "pallete"));
                    spriteNo += commandLineParams.ImagenNGP.Ancho;
                }

                spriteNo = x + 1;


            }

            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
        }
    }
}
