Imports System.Collections.ObjectModel
Imports System.Reflection

Public Class SpriteLoader

    Public Shared Function LoadSprites(ByVal resourceClass As Type) As SpriteCollection
        Return LoadSprites(resourceClass, Color.Empty)
    End Function

    Public Shared Function LoadSprites(ByVal resourceClass As Type, ByVal transparentColor As Color) As SpriteCollection

        Dim props = resourceClass.GetProperties(Reflection.BindingFlags.Static Or Reflection.BindingFlags.NonPublic)
        Dim lst As New SpriteCollection

        'Get all the images from the resource class
        For Each p As PropertyInfo In props

            If p.PropertyType Is GetType(Bitmap) Then
                Dim image As Bitmap = p.GetValue(Nothing, Nothing)

                If Not transparentColor = Color.Empty Then
                    image.MakeTransparent(transparentColor)
                End If

                lst.Add(New Sprite(p.Name, image))
            End If

        Next

        Return lst
    End Function

End Class
