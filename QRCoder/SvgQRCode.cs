﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace QRCoder
{
    public class SvgQRCode : AbstractQRCode<string>, IDisposable
    {
        public SvgQRCode(QRCodeData data) : base(data) { }


        public override string GetGraphic(int pixelsPerModule)
        {
            Size viewBox = new Size(pixelsPerModule*qrCodeData.ModuleMatrix.Count, pixelsPerModule * qrCodeData.ModuleMatrix.Count);
            return GetGraphic(viewBox, Color.Black, Color.White);
        }

        public string GetGraphic(Size viewBox)
        {
            return GetGraphic(viewBox, Color.Black, Color.White);
        }

        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor)
        {
            return GetGraphic(viewBox, ColorTranslator.ToHtml(Color.FromArgb(darkColor.ToArgb())), ColorTranslator.ToHtml(Color.FromArgb(lightColor.ToArgb())));
        }

        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            return GetGraphic(viewBox, ColorTranslator.ToHtml(Color.FromArgb(darkColor.ToArgb())), ColorTranslator.ToHtml(Color.FromArgb(lightColor.ToArgb())), drawQuietZones);
        }

        public string GetGraphic(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true)
        {
            StringBuilder svgFile = new StringBuilder(@"<svg version=""1.1"" baseProfile=""full"" width="""+viewBox.Width+ @""" height="""+viewBox.Height+ @""" xmlns=""http://www.w3.org/2000/svg"">");
            int unitsPerModule = (int)Math.Floor(Convert.ToDouble(Math.Min(viewBox.Width, viewBox.Height)) / qrCodeData.ModuleMatrix.Count);
            var size = (qrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * unitsPerModule;
            int offset = drawQuietZones ? 0 : 4 * unitsPerModule;
            int drawableSize = size + offset;
            for (int x = 0; x < drawableSize; x = x + unitsPerModule)
            {
                for (int y = 0; y < drawableSize; y = y + unitsPerModule)
                {
                    var module = qrCodeData.ModuleMatrix[(y + unitsPerModule) / unitsPerModule - 1][(x + unitsPerModule) / unitsPerModule - 1];
                    svgFile.AppendLine(@"<rect x="""+(x - offset)+ @""" y=""" + (y - offset) + @""" width=""" + unitsPerModule + @""" height=""" + unitsPerModule + @""" fill=""" + (module ? darkColorHex : lightColorHex) + @""" />");
                }
            }
            svgFile.Append(@"</svg>");
            return svgFile.ToString();
        }

        public void Dispose()
        {
            this.qrCodeData = null;
        }
    }
}
