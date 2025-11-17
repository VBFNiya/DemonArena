Option Strict On

Imports System.Runtime.CompilerServices

Public Module ExtensionMethods

    <Extension()> _
    Public Function GetCenter(ByVal rect As RectangleF) As PointF

        Return New PointF(rect.X + (rect.Width / 2),
                          rect.Y + (rect.Height / 2))

    End Function


    <Extension()> _
    Public Function ToPoint(ByVal pt As PointF) As Point
        Return New Point(CInt(pt.X), CInt(pt.Y))
    End Function

    <Extension()> _
    Public Function ToPointF(ByVal pt As Point) As PointF
        Return New PointF(CSng(pt.X), CSng(pt.Y))
    End Function

    <Extension()> _
    Public Function ToRectangle(ByVal rect As RectangleF) As Rectangle
        Return New Rectangle(CInt(rect.X),
                             CInt(rect.Y),
                             CInt(rect.Width),
                             CInt(rect.Height))

    End Function

    <Extension()> _
    Public Function ToRectangleF(ByVal rect As Rectangle) As RectangleF
        Return New RectangleF(CSng(rect.X),
                     CSng(rect.Y),
                     CSng(rect.Width),
                     CSng(rect.Height))

    End Function

    <Extension()> _
    Public Sub CenterRectInRect(ByRef rect As RectangleF, ByVal inRect As RectangleF)
        rect.X = inRect.X + (inRect.Width / 2) - (rect.Width / 2)
        rect.Y = inRect.Y + (inRect.Height / 2) - (rect.Height / 2)
    End Sub


End Module
