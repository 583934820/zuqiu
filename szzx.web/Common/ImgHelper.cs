using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace szzx.web.Common
{
    public static class ImgHelper
    {
        public static  void GenerateImg(PlayerImgModel model, List<string> imgPaths)
        {
            try
            {

                var width = 285;
                var height = 168;

                var bitmap = new Bitmap(width, height);
                var graphics = Graphics.FromImage(bitmap);

                //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                graphics.Clear(Color.FromArgb(255, 255, 255));

                if (!string.IsNullOrEmpty(model.ImgPath) && File.Exists(model.ImgPath))
                {
                    var img = new Bitmap(model.ImgPath);
                    var rectangle = new Rectangle(10, 30, 70, 106);

                    //画图
                    graphics.DrawImage(img, rectangle, new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                }


                //定义画刷
                Color col = Color.FromArgb(51, 51, 51);
                var brush = new SolidBrush(col);

                Color greenCol = Color.FromArgb(67, 228, 65);
                var greenBrush = new SolidBrush(greenCol);

                var font = new Font("微软雅黑", 8f, FontStyle.Bold);

                graphics.DrawString("姓    名：", font, greenBrush, 95f, 25f);
                graphics.DrawString(model.Name ?? "", font, brush, 147f, 25f);

                graphics.DrawString("年    龄：", font, greenBrush, 205f, 25f);
                graphics.DrawString(model.Age.ToString(), font, brush, 250f, 25f);

                graphics.DrawString("会员编号：", font, greenBrush, 95f, 60f);
                graphics.DrawString(model.VipNo ?? "", font, brush, 147f, 60f);

                graphics.DrawString("身份证号：", font, greenBrush, 95f, 95f);
                graphics.DrawString(model.CardNo ?? "", font, brush, 147f, 95f);

                graphics.DrawString("所在球队：", font, greenBrush, 95f, 125f);
                graphics.DrawString("球衣号码：", font, greenBrush, 205f, 125f);
                graphics.DrawString(model.TeamName ?? "", font, brush, 95f, 140f);
                graphics.DrawString(model.PlayerNo ?? "", font, brush, 260f, 125f);

                var path = AppDomain.CurrentDomain.BaseDirectory; ;
                var distPath = path + @"\upload\zipFile\" +  "team" + model.TeamId.ToString();
                if (!Directory.Exists(distPath))
                {
                    Directory.CreateDirectory(distPath);
                }
                var fileName = distPath + "\\" + model.VipNo + ".jpg";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                bitmap.Save(fileName);

                imgPaths.Add(fileName);
            }
            catch (Exception ex)
            {

            }
            
        }
    }

    public class PlayerImgModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string VipNo { get; set; }
        public string CardNo { get; set; }
        public string TeamName { get; set; }
        public int TeamId { get; set; }
        public string PlayerNo { get; set; }
        public string ImgPath { get; set; }
    }
}