using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace NGP_Bitmap_2_File.Core.Clases
{
    class NGPImage
    {
        private CLParams CommandLineaParams { get; set; }
        public List<string> HexRows { get; set; }

        /// <summary>
        /// Indica si la imagen procesada, ha generado filas de valores hexadecimales.
        /// </summary>
        /// <returns></returns>
        public bool HasHexRows()
        {
            return this.HexRows.Count > 0;
        }

        public NGPImage(CLParams commandlineParams)
        {
            if (!commandlineParams.IsValidParams) { return; }

            this.CommandLineaParams = commandlineParams;

            GenerateHexNGPImage();
        }

        public void GenerateHexNGPImage()
        {
            //GetHexPixels(CommandLineaParams.TiledImage);
            GetHexPixelsV2(CommandLineaParams.ImagenNGP.TiledImage);
        }

        public static byte[] ImageToByteArray(Bitmap imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Obtiene la lista de filas y los valores hexadecimales de los pixeles. Se lee y escribe de izquierda a derecha (por filas)
        /// </summary>
        /// <param name="imageIn"></param>
        private void GetHexPixels(Bitmap imageIn)
        {
            const string DIF = "0x";
            string hexColor;
            int x;
            int y;

            HexRows = new List<string>();

            for (y = 0; y < imageIn.Height; y++)
            {
                int hexNumber = 0;
                hexColor = DIF;

                for (x = 0; x < imageIn.Width; x++)
                {
                    Color pxColor = imageIn.GetPixel(x, y);

                    if (x != 0 && Utils.IsPar(x))
                    {
                        hexNumber = 0;
                    }

                    if (!Utils.IsPar(x))
                    {
                        hexNumber += GetEven(pxColor, CommandLineaParams);
                    }
                    else
                    {
                        hexNumber = GetOdd(pxColor, CommandLineaParams);
                    }

                    if (!Utils.IsPar(x))
                    {
                        hexColor += hexNumber.ToString("X").ToLower();
                    }

                    if ((x + 1) >= 8 && Utils.IsMultiploDe((x + 1), 8))
                    {
                        HexRows.Add(hexColor);
                        hexColor = DIF;
                        hexNumber = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene la lista de filas y los valores hexadecimales de los pixeles. Se lee y escribe de arriba a abajo (por columnas)
        /// </summary>
        /// <param name="imageIn"></param>
        private void GetHexPixelsV2(Bitmap imageIn)
        {
            const string DIF = "0x";
            string hexColor;
            int x;
            int y;
            short currentCol = 0;

            HexRows = new List<string>();

            for (y = 0; y < imageIn.Height; y++)
            {
                int hexNumber = 0;
                hexColor = DIF;

                for (x = (currentCol * 8); x < ((currentCol + 1) * 8); x++)
                {
                    Color pxColor = imageIn.GetPixel(x, y);

                    if (x != 0 && Utils.IsPar(x))
                    {
                        hexNumber = 0;
                    }

                    if (!Utils.IsPar(x))
                    {
                        hexNumber += GetEven(pxColor, CommandLineaParams);
                    }
                    else
                    {
                        hexNumber = GetOdd(pxColor, CommandLineaParams);
                    }

                    if (!Utils.IsPar(x))
                    {
                        hexColor += hexNumber.ToString("X").ToLower();
                    }

                    if ((x + 1) >= 8 && Utils.IsMultiploDe((x + 1), 8))
                    {
                        HexRows.Add(hexColor);
                        hexColor = DIF;
                        hexNumber = 0;
                    }
                }

                if (y == (imageIn.Height - 1))
                {
                    currentCol++;
                    if (currentCol < (imageIn.Width / 8)) { y = -1; }
                }
            }
        }

        /// <summary>
        /// Obtiene el valor numérico del Color impar
        /// </summary>
        /// <param name="pxColor"></param>
        /// <param name="clParams"></param>
        /// <returns></returns>
        private int GetOdd(Color pxColor, CLParams clParams)
        {
            int value = 0;

            if (pxColor == clParams.Color1)
            {
                value = 4;
            }

            if (pxColor == clParams.Color2)
            {
                value = 8;
            }

            if (pxColor == clParams.Color3)
            {
                value = 12;
            }

            return value;
        }

        /// <summary>
        /// Obtiene el valor numérico del Color par
        /// </summary>
        /// <param name="pxColor"></param>
        /// <param name="clParams"></param>
        /// <returns></returns>
        private int GetEven(Color pxColor, CLParams clParams)
        {
            int value = 0;

            if (pxColor == clParams.Color1)
            {
                value = 1;
            }

            if (pxColor == clParams.Color2)
            {
                value = 2;
            }

            if (pxColor == clParams.Color3)
            {
                value = 3;
            }

            return value;
        }
    }
}
