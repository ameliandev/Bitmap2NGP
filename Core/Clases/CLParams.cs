using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel;

namespace NGP_Bitmap_2_File.Core.Clases
{
    class Imagen
    {
        public int Ancho { get; set; }
        public int Alto { get; set; }
        public Bitmap TiledImage { get; set; }
    }

    class CLParams
    {
        //var c1 = Color.FromArgb(0,255,0,255);

        #region Propiedades
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public Color Color3 { get; set; }
        //public Bitmap TiledImage { get; set; }
        public Imagen ImagenNGP { get; set; }
        public string Plane { get; set; }
        public bool IsValidParams { get; set; }
        public bool GenerarInstall { get; set; }
        public bool GenerarInstallOnScreen { get; set; }
        public bool GenerarPaleta { get; set; }
        public List<string> ErrorMessage { get; set; }

        private string _argsPath = string.Empty;
        private string _argsCol1 = string.Empty;
        private string _argsCol2 = string.Empty;
        private string _argsCol3 = string.Empty;
        private string _argsPln = string.Empty;
        private string _argsOpt = string.Empty;

        [DefaultValue("")]
        public string ArgsPath { get { return _argsPath; } set { _argsPath = value; } }
        [DefaultValue("")]
        public string ArgsColor1 { get { return _argsCol1; } set { _argsCol1 = value; } }
        [DefaultValue("")]
        public string ArgsColor2 { get { return _argsCol2; } set { _argsCol2 = value; } }
        [DefaultValue("")]
        public string ArgsColor3 { get { return _argsCol3; } set { _argsCol3 = value; } }
        [DefaultValue("")]
        public string ArgsPlane { get { return _argsPln; } set { _argsPln = value; } }
        [DefaultValue("")]
        public string ArgsOptions { get { return _argsOpt; } set { _argsOpt = value; } }

        #endregion

        private void SetParamsByArguments(string[] arguments)
        {
            int index = 0;

            foreach (var argument in arguments)
            {
                switch (index)
                {
                    case 0:
                        if (Utils.IsOption(argument))
                        {
                            ArgsOptions = argument;
                        } else if (Utils.IsPath(argument))
                        {
                            ArgsPath = argument;
                        }
                        break;
                    case 1:
                        if (Utils.IsPath(argument))
                        {
                            ArgsPath = argument;
                        } else if (Utils.IsNGPPlane(argument))
                        {
                            ArgsPlane = argument;
                        }
                        break;
                    case 2:
                        if (Utils.IsNGPPlane(argument))
                        {
                            ArgsPlane = argument;
                        } else if (Utils.IsRGBColor(argument))
                        {
                            ArgsColor1 = argument;
                        }
                        break;
                    case 3:
                    case 4:
                        if (Utils.IsRGBColor(argument))
                        {
                            switch (index)
                            {
                                case 3:
                                    if (!ArgsColor1.Equals(string.Empty))
                                    {
                                        ArgsColor2 = argument;
                                    } else
                                    {
                                        ArgsColor1 = argument;
                                    }
                                    break;
                                case 4:
                                    if (!ArgsColor2.Equals(string.Empty))
                                    {
                                        ArgsColor3 = argument;
                                    } else
                                    {
                                        ArgsColor2 = argument;
                                    }
                                    break;
                                case 5:
                                    ArgsColor3 = argument;
                                    break;
                            }
                        } 
                        break;
                    case 5:
                        ArgsColor3 = argument;
                        break;
                }

                index++;
            }
        }

        //public CLParams(string imagePath, string hexColor1, string hexColor2, string hexColor3, string planeNo, string optionInstall = "")
        public CLParams(string[] arguments)
        {
            SetParamsByArguments(arguments);

            ErrorMessage = new List<string>();

            this.IsValidParams = Validate();

            if (!this.IsValidParams) { return; }

            this.ImagenNGP = new Imagen();

            this.Color1 = Color.FromArgb(255, int.Parse(ArgsColor1.Split(',')[0]), int.Parse(ArgsColor1.Split(',')[1]), int.Parse(ArgsColor1.Split(',')[2])); // ColorTranslator.FromHtml(hexColor1);

            if (!ArgsColor2.Equals(string.Empty) && !ArgsColor2.Equals("0"))
            {
                this.Color2 = Color.FromArgb(255, int.Parse(ArgsColor2.Split(',')[0]), int.Parse(ArgsColor2.Split(',')[1]), int.Parse(ArgsColor2.Split(',')[2])); //ColorTranslator.FromHtml(hexColor2);
            }

            if (!ArgsColor3.Equals(string.Empty) && !ArgsColor3.Equals("0"))
            {
                this.Color3 = Color.FromArgb(255, int.Parse(ArgsColor3.Split(',')[0]), int.Parse(ArgsColor3.Split(',')[1]), int.Parse(ArgsColor3.Split(',')[2])); //ColorTranslator.FromHtml(hexColor3);
            }

            var tmpImage = GetTmpImage(ArgsPath);
            byte[] msImage = NGPImage.ImageToByteArray(tmpImage);
            tmpImage.Dispose();
            
            using (var ms = new MemoryStream(msImage))
            {
                this.ImagenNGP.TiledImage = new Bitmap(ms);
                this.ImagenNGP.Ancho = this.ImagenNGP.TiledImage.Width / 8;
                this.ImagenNGP.Alto = this.ImagenNGP.TiledImage.Height / 8;
            }

            this.Plane = getScreenPlaneByParam(ArgsPlane);

            #region Opciones

            if (!ArgsOptions.Equals(string.Empty) && ArgsOptions.Contains('-'))
            {
                foreach (char option in ArgsOptions)
                {
                    if (option.Equals('-')) { continue; }

                    if (option.Equals('p')) { this.GenerarPaleta = true; }
                    if (option.Equals('i')) { this.GenerarInstall = true; }
                    if (option.Equals('d')) { this.GenerarInstallOnScreen = true; }
                }
            }
            
            #endregion
        }

        private bool Validate()
        {
            if (ArgsPath.Equals(string.Empty) && !File.Exists(ArgsPath))
            {
                if (ArgsPath.Equals(string.Empty))
                {
                    this.ErrorMessage.Add("first parameter, Image path, not established.");
                } else
                {
                    this.ErrorMessage.Add("the image path doesn't exists or is not a file.");
                }
            } else
            {
                var fileName = ArgsPath.ToUpper();

                if (!fileName.Contains("PNG") && !fileName.Contains("PCX") && !fileName.Contains("GIF"))
                {
                    this.ErrorMessage.Add("not compatible image format. Accepted PNG, GIF or PCX.");
                }
            }

            if ((ArgsPlane.Equals(string.Empty)) || (!ArgsPlane.Equals("PS") && !ArgsPlane.Equals("P1") && !ArgsPlane.Equals("P2")))
            {
                this.ErrorMessage.Add("screen plane not established.");
            }

            if (ArgsColor1.Equals(string.Empty))
            {
                this.ErrorMessage.Add("at least one color is required");
            }

            if (!ArgsColor2.Equals(string.Empty) && !Utils.IsRGBColor(ArgsColor2))
            {
                this.ErrorMessage.Add("Color 2 is not a RGB color.");
            }

            if (!ArgsColor3.Equals(string.Empty) && !Utils.IsRGBColor(ArgsColor3))
            {
                this.ErrorMessage.Add("Color 3 is not a RGB color.");
            }

            if (!ArgsOptions.Equals(string.Empty) && !Utils.IsOption(ArgsOptions)) {
                this.ErrorMessage.Add(string.Format("options {0} not recognized.", ArgsOptions));
            } else
            {
                foreach (char option in ArgsOptions)
                {
                    if (option.Equals('-')) { continue; }

                    if (!option.Equals('p') && !option.Equals('i') && !option.Equals('d')) {
                        this.ErrorMessage.Add(string.Format("option '{0}' not recognized.", option));
                    }
                }
            }

            return (ErrorMessage.Count > 0) ? false : true;
        }

        /// <summary>
        /// Indica si el color es de tipo hexadecimal para HTML
        /// </summary>
        /// <param name="strhex"></param>
        /// <returns></returns>
        private bool IsHexColor(string strhex)
        {
            try
            {
                if (strhex.Equals(string.Empty)) { return false; }

                Regex rx1 = new Regex("([#]{1})([0-Z0-z]{6})", RegexOptions.Compiled);
                Regex rx2 = new Regex("([0-Z0-z]{6})|([0-Z0-z]{8})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                MatchCollection matches1 = rx1.Matches(strhex);
                MatchCollection matches2 = rx2.Matches(strhex);

                return (matches1.Count > 0 ? true : matches2.Count > 0);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Obtiene una copia en memoria de la imagen para no bloquear el fichero original
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private static Bitmap GetTmpImage(string imagePath)
        {
            return new Bitmap(imagePath);
        }

        private string getScreenPlaneByParam(string param)
        {
            string formatedPlane = string.Empty;

            switch (param)
            {
                case "P1":
                    formatedPlane = "SCR_1_PLANE";
                    break;
                case "P2":
                    formatedPlane = "SCR_2_PLANE";
                    break;
                case "PS":
                    formatedPlane = "SPRITE_PLANE";
                    break;
            }

            return formatedPlane;
        }
    }
}
