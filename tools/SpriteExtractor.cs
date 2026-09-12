using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AnnoyingCat.Tools
{
    public class SpriteExtractor
    {
        public static void Main(string[] args)
        {
            string baseDir = Directory.GetCurrentDirectory();
            if (args.Length > 0 && Directory.Exists(args[0]))
            {
                baseDir = args[0];
            }

            Console.WriteLine("Clean Morphological SpriteExtractor starting in: " + baseDir);
            string assetsDir = Path.Combine(baseDir, "assets", "sprites");
            Directory.CreateDirectory(Path.Combine(assetsDir, "walk"));
            Directory.CreateDirectory(Path.Combine(assetsDir, "idle"));
            Directory.CreateDirectory(Path.Combine(assetsDir, "dance"));
            Directory.CreateDirectory(Path.Combine(assetsDir, "mouth"));
            Directory.CreateDirectory(Path.Combine(assetsDir, "actions"));

            string walkSheet = Path.Combine(baseDir, "WhatsApp Image 2026-09-12 at 00.23.41.jpeg");
            string actionSheet = Path.Combine(baseDir, "WhatsApp Image 2026-09-12 at 00.23.40.jpeg");
            string expSheet = Path.Combine(baseDir, "poocha exp.jpg");
            string mainCat = Path.Combine(baseDir, "WhatsApp Image 2026-09-12 at 00.23.39.jpeg");

            // 1. Process Walk Cycle (6 frames)
            if (File.Exists(walkSheet))
            {
                Console.WriteLine("Processing Walk Cycle...");
                using (Bitmap bmp = new Bitmap(walkSheet))
                {
                    Rectangle[] walkRects = new Rectangle[]
                    {
                        new Rectangle(60, 218, 275, 260),    // walk_0
                        new Rectangle(368, 218, 280, 260),   // walk_1
                        new Rectangle(684, 218, 285, 260),   // walk_2
                        new Rectangle(64, 584, 272, 262),    // walk_3
                        new Rectangle(368, 584, 280, 262),   // walk_4
                        new Rectangle(684, 584, 285, 262)    // walk_5
                    };

                    for (int i = 0; i < walkRects.Length; i++)
                    {
                        string outPath = Path.Combine(assetsDir, "walk", "walk_" + i + ".png");
                        ExtractCleanAndNormalize(bmp, walkRects[i], outPath, 300, 280, true);
                        Console.WriteLine("Saved: " + outPath);
                    }
                }
            }

            // 2. Process Action Sheet
            if (File.Exists(actionSheet))
            {
                Console.WriteLine("Processing Action Sheet...");
                using (Bitmap bmp = new Bitmap(actionSheet))
                {
                    ExtractCleanAndNormalize(bmp, new Rectangle(88, 212, 235, 265), Path.Combine(assetsDir, "idle", "idle_look_up.png"), 300, 280, true);
                    ExtractCleanAndNormalize(bmp, new Rectangle(58, 545, 280, 310), Path.Combine(assetsDir, "dance", "dance_0.png"), 300, 310, true);
                    ExtractCleanAndNormalize(bmp, new Rectangle(382, 542, 295, 305), Path.Combine(assetsDir, "actions", "pounce.png"), 300, 310, true);
                    ExtractCleanAndNormalize(bmp, new Rectangle(685, 595, 275, 258), Path.Combine(assetsDir, "actions", "sit_paw.png"), 300, 280, true);
                }
            }

            // 3. Process Poocha Exp Sheet
            if (File.Exists(expSheet))
            {
                Console.WriteLine("Processing Expression Sheet...");
                using (Bitmap bmp = new Bitmap(expSheet))
                {
                    ExtractCleanAndNormalize(bmp, new Rectangle(40, 68, 185, 212), Path.Combine(assetsDir, "idle", "idle_happy.png"), 300, 280, true);
                    ExtractCleanAndNormalize(bmp, new Rectangle(275, 80, 225, 205), Path.Combine(assetsDir, "dance", "dance_1.png"), 300, 280, true);
                    ExtractCleanAndNormalize(bmp, new Rectangle(42, 345, 210, 265), Path.Combine(assetsDir, "idle", "idle_curious.png"), 300, 280, true);
                    ExtractCleanAndNormalize(bmp, new Rectangle(275, 388, 220, 230), Path.Combine(assetsDir, "actions", "surprised.png"), 300, 280, true);
                    ExtractCleanAndNormalize(bmp, new Rectangle(38, 705, 215, 255), Path.Combine(assetsDir, "actions", "playful.png"), 300, 280, true);
                    ExtractCleanAndNormalize(bmp, new Rectangle(275, 695, 215, 265), Path.Combine(assetsDir, "idle", "idle_proud.png"), 300, 280, true);

                    // Mouth frames
                    Rectangle[] mouthRects = new Rectangle[]
                    {
                        new Rectangle(532, 75, 220, 185),
                        new Rectangle(782, 75, 220, 185),
                        new Rectangle(532, 375, 220, 185),
                        new Rectangle(782, 375, 220, 185),
                        new Rectangle(532, 695, 220, 225),
                        new Rectangle(782, 695, 220, 225)
                    };

                    for (int i = 0; i < mouthRects.Length; i++)
                    {
                        string outPath = Path.Combine(assetsDir, "mouth", "mouth_" + i + ".png");
                        ExtractCleanAndNormalize(bmp, mouthRects[i], outPath, 260, 250, false);
                        Console.WriteLine("Saved: " + outPath);
                    }
                }
            }

            // 4. Main Cat Stare Portrait
            if (File.Exists(mainCat))
            {
                Console.WriteLine("Processing Main Cat Stare Portrait...");
                using (Bitmap bmp = new Bitmap(mainCat))
                {
                    Rectangle mainRect = new Rectangle(220, 160, 810, 890);
                    string outPath = Path.Combine(assetsDir, "idle", "idle_stare.png");
                    ExtractCleanAndNormalize(bmp, mainRect, outPath, 300, 280, true);
                    Console.WriteLine("Saved: " + outPath);

                    string iconPath = Path.Combine(assetsDir, "icon.ico");
                    CreateIconFromBitmap(bmp, new Rectangle(250, 150, 750, 750), iconPath);
                }
            }

            Console.WriteLine("Sprite extraction complete!");
        }

        public static void ExtractCleanAndNormalize(Bitmap src, Rectangle cropRect, string outPath, int canvasW, int canvasH, bool bottomAlign)
        {
            int cx = Math.Max(0, cropRect.X);
            int cy = Math.Max(0, cropRect.Y);
            int cw = Math.Min(src.Width - cx, cropRect.Width);
            int ch = Math.Min(src.Height - cy, cropRect.Height);

            using (Bitmap cropped = new Bitmap(cw, ch, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(cropped))
                {
                    g.DrawImage(src, new Rectangle(0, 0, cw, ch), new Rectangle(cx, cy, cw, ch), GraphicsUnit.Pixel);
                }

                Color bg = cropped.GetPixel(2, 2);

                // Step 1: Detect line art / colored drawing pixels
                bool[,] isLineArt = new bool[cw, ch];
                for (int y = 0; y < ch; y++)
                {
                    for (int x = 0; x < cw; x++)
                    {
                        Color p = cropped.GetPixel(x, y);
                        int dr = p.R - bg.R;
                        int dg = p.G - bg.G;
                        int db = p.B - bg.B;
                        double dist = Math.Sqrt(dr * dr + dg * dg + db * db);
                        if (dist > 28 || (p.R - p.B) > 28)
                        {
                            isLineArt[x, y] = true;
                        }
                    }
                }

                // Step 2: Dilation (radius 2) to bridge tiny 1-2px pencil gaps
                bool[,] dilated = new bool[cw, ch];
                int radius = 2;
                for (int y = 0; y < ch; y++)
                {
                    for (int x = 0; x < cw; x++)
                    {
                        if (isLineArt[x, y])
                        {
                            for (int dy = -radius; dy <= radius; dy++)
                            {
                                for (int dx = -radius; dx <= radius; dx++)
                                {
                                    int nx = x + dx, ny = y + dy;
                                    if (nx >= 0 && nx < cw && ny >= 0 && ny < ch)
                                    {
                                        dilated[nx, ny] = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // Step 3: Flood fill from borders
                bool[,] isBg = new bool[cw, ch];
                Queue<Point> queue = new Queue<Point>();
                for (int x = 0; x < cw; x++)
                {
                    if (!dilated[x, 0]) queue.Enqueue(new Point(x, 0));
                    if (!dilated[x, ch - 1]) queue.Enqueue(new Point(x, ch - 1));
                }
                for (int y = 0; y < ch; y++)
                {
                    if (!dilated[0, y]) queue.Enqueue(new Point(0, y));
                    if (!dilated[cw - 1, y]) queue.Enqueue(new Point(cw - 1, y));
                }

                while (queue.Count > 0)
                {
                    Point pt = queue.Dequeue();
                    int x = pt.X, y = pt.Y;
                    if (x < 0 || x >= cw || y < 0 || y >= ch) continue;
                    if (isBg[x, y] || dilated[x, y]) continue;

                    isBg[x, y] = true;
                    if (x > 0 && !isBg[x - 1, y] && !dilated[x - 1, y]) queue.Enqueue(new Point(x - 1, y));
                    if (x < cw - 1 && !isBg[x + 1, y] && !dilated[x + 1, y]) queue.Enqueue(new Point(x + 1, y));
                    if (y > 0 && !isBg[x, y - 1] && !dilated[x, y - 1]) queue.Enqueue(new Point(x, y - 1));
                    if (y < ch - 1 && !isBg[x, y + 1] && !dilated[x, y + 1]) queue.Enqueue(new Point(x, y + 1));
                }

                // Step 4: Expand background back into dilated border pixels where colors match paper
                for (int y = 0; y < ch; y++)
                {
                    for (int x = 0; x < cw; x++)
                    {
                        if (dilated[x, y])
                        {
                            Color p = cropped.GetPixel(x, y);
                            int dr = p.R - bg.R;
                            int dg = p.G - bg.G;
                            int db = p.B - bg.B;
                            double dist = Math.Sqrt(dr * dr + dg * dg + db * db);
                            if (dist < 26)
                            {
                                bool nearTrueBg = false;
                                for (int dy = -radius; dy <= radius && !nearTrueBg; dy++)
                                {
                                    for (int dx = -radius; dx <= radius && !nearTrueBg; dx++)
                                    {
                                        int nx = x + dx, ny = y + dy;
                                        if (nx >= 0 && nx < cw && ny >= 0 && ny < ch && isBg[nx, ny])
                                        {
                                            nearTrueBg = true;
                                        }
                                    }
                                }
                                if (nearTrueBg) isBg[x, y] = true;
                            }
                        }
                    }
                }

                // Step 5: Render cutout with soft antialiasing
                using (Bitmap cutout = new Bitmap(cw, ch, PixelFormat.Format32bppArgb))
                {
                    for (int y = 0; y < ch; y++)
                    {
                        for (int x = 0; x < cw; x++)
                        {
                            if (isBg[x, y])
                            {
                                cutout.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                            }
                            else
                            {
                                Color p = cropped.GetPixel(x, y);
                                int dr = p.R - bg.R;
                                int dg = p.G - bg.G;
                                int db = p.B - bg.B;
                                double dist = Math.Sqrt(dr * dr + dg * dg + db * db);

                                bool nearBg = false;
                                for (int dy = -1; dy <= 1 && !nearBg; dy++)
                                {
                                    for (int dx = -1; dx <= 1 && !nearBg; dx++)
                                    {
                                        int nx = x + dx, ny = y + dy;
                                        if (nx >= 0 && nx < cw && ny >= 0 && ny < ch && isBg[nx, ny])
                                        {
                                            nearBg = true;
                                        }
                                    }
                                }

                                if (nearBg && dist < 32)
                                {
                                    int alpha = (int)Math.Min(255, Math.Max(0, (dist / 32.0) * 255));
                                    cutout.SetPixel(x, y, Color.FromArgb(alpha, p.R, p.G, p.B));
                                }
                                else
                                {
                                    cutout.SetPixel(x, y, p);
                                }
                            }
                        }
                    }

                    // Step 6: Paw Shadow Cleanup (clean faint shadow pixels below paw bottom)
                    int minX = cw, minY = ch, maxX = 0, maxY = 0;
                    for (int y = 0; y < ch; y++)
                    {
                        for (int x = 0; x < cw; x++)
                        {
                            if (cutout.GetPixel(x, y).A > 30)
                            {
                                if (x < minX) minX = x;
                                if (x > maxX) maxX = x;
                                if (y < minY) minY = y;
                                if (y > maxY) maxY = y;
                            }
                        }
                    }

                    if (minX > maxX || minY > maxY)
                    {
                        cutout.Save(outPath, ImageFormat.Png);
                        return;
                    }

                    if (bottomAlign)
                    {
                        int shadowCutoff = maxY - 7;
                        for (int y = shadowCutoff; y <= maxY; y++)
                        {
                            for (int x = minX; x <= maxX; x++)
                            {
                                Color p = cutout.GetPixel(x, y);
                                if (p.A > 0 && p.A < 220)
                                {
                                    cutout.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                                }
                            }
                        }

                        // Recompute bounding box
                        minX = cw; minY = ch; maxX = 0; maxY = 0;
                        for (int y = 0; y < ch; y++)
                        {
                            for (int x = 0; x < cw; x++)
                            {
                                if (cutout.GetPixel(x, y).A > 30)
                                {
                                    if (x < minX) minX = x;
                                    if (x > maxX) maxX = x;
                                    if (y < minY) minY = y;
                                    if (y > maxY) maxY = y;
                                }
                            }
                        }
                    }

                    int spriteW = maxX - minX + 1;
                    int spriteH = maxY - minY + 1;

                    // Step 7: Normalize to uniform canvas
                    using (Bitmap normalized = new Bitmap(canvasW, canvasH, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics ng = Graphics.FromImage(normalized))
                        {
                            ng.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            ng.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                            double scale = 1.0;
                            double maxAllowedW = canvasW - 20;
                            double maxAllowedH = canvasH - 20;
                            if (spriteW > maxAllowedW || spriteH > maxAllowedH)
                            {
                                scale = Math.Min(maxAllowedW / spriteW, maxAllowedH / spriteH);
                            }

                            int drawW = (int)(spriteW * scale);
                            int drawH = (int)(spriteH * scale);
                            int destX = (canvasW - drawW) / 2;
                            int destY = bottomAlign ? (canvasH - drawH - 10) : ((canvasH - drawH) / 2);

                            ng.DrawImage(cutout, new Rectangle(destX, destY, drawW, drawH), new Rectangle(minX, minY, spriteW, spriteH), GraphicsUnit.Pixel);
                        }

                        normalized.Save(outPath, ImageFormat.Png);
                    }
                }
            }
        }

        public static void CreateIconFromBitmap(Bitmap src, Rectangle cropRect, string outIconPath)
        {
            using (Bitmap face = new Bitmap(cropRect.Width, cropRect.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(face))
                {
                    g.DrawImage(src, new Rectangle(0, 0, cropRect.Width, cropRect.Height), cropRect, GraphicsUnit.Pixel);
                }

                using (Bitmap iconBmp = new Bitmap(48, 48, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(iconBmp))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(face, new Rectangle(0, 0, 48, 48));
                    }

                    using (FileStream fs = new FileStream(outIconPath, FileMode.Create))
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write((short)0);
                        bw.Write((short)1);
                        bw.Write((short)1);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            iconBmp.Save(ms, ImageFormat.Png);
                            byte[] pngBytes = ms.ToArray();

                            bw.Write((byte)48);
                            bw.Write((byte)48);
                            bw.Write((byte)0);
                            bw.Write((byte)0);
                            bw.Write((short)1);
                            bw.Write((short)32);
                            bw.Write((int)pngBytes.Length);
                            bw.Write((int)22);

                            bw.Write(pngBytes);
                        }
                    }
                }
            }
        }
    }
}
