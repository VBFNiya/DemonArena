Imports System.Drawing.Drawing2D
Imports Microsoft.Xna.Framework.Graphics

Public Class DrawingHelpers

    Public Shared Function CreateRadialGradient(ByVal gradColor As Color, ByVal size As Size) As Bitmap
        Dim b As New Bitmap(size.Width, size.Height)

        Using g As Graphics = Graphics.FromImage(b)

            Using path As New GraphicsPath()
                ' Create a path that consists of a single ellipse. 
                path.AddEllipse(0, 0, size.Width, size.Height)

                ' Use the path to construct a brush. 
                Using pthGrBrush As New PathGradientBrush(path)

                    ' Set the color at the center of the path to blue.
                    pthGrBrush.CenterColor = gradColor 'Color.FromArgb(255, 0, 0, 255)

                    ' Set the color along the entire boundary  
                    ' of the path to transparent. 
                    Dim colors As Color() = {Color.Transparent}
                    pthGrBrush.SurroundColors = colors

                    g.FillEllipse(pthGrBrush, 0, 0, size.Width, size.Height)
                End Using
            End Using
        End Using

        Return b
    End Function

    'Converts bitmaps to texture2d
    'Additional Credit: dday9 @ VBForums
    Public Shared Function BitmapToTexture2D(ByVal GraphicsDevice As GraphicsDevice, ByVal image As System.Drawing.Bitmap) As Texture2D
        Dim bufferSize As Integer = image.Height * image.Width * 4

        Dim memoryStream As New System.IO.MemoryStream(bufferSize)
        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png)

        memoryStream.Seek(0, IO.SeekOrigin.Begin)
        Dim texture As Texture2D = Texture2D.FromStream(GraphicsDevice, memoryStream, image.Width, image.Height, False)

        memoryStream.Close()
        Return texture
    End Function





End Class
