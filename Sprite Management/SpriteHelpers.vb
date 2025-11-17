Imports System.Text

Public Class SpriteHelpers

    Public Shared Function GetDirectionFromAngle(ByVal angle As Double) As SpriteDirection
        Dim d As SpriteDirection

        Dim range As Double = 45

        If angle = 0 Then d = SpriteDirection.Right
        If angle = 180 Then d = SpriteDirection.Left
        If angle = 90 Then d = SpriteDirection.Down
        If angle = 270 Then d = SpriteDirection.Up

        If angle > 0 AndAlso angle < 90 Then d = SpriteDirection.LowerRight
        If angle > 90 AndAlso angle < 180 Then d = SpriteDirection.LowerLeft
        If angle > 180 AndAlso angle < 270 Then d = SpriteDirection.UpperLeft
        If angle > 270 Then d = SpriteDirection.UpperRight

        If angle > 90 - (range / 2) AndAlso angle < 90 + (range / 2) Then d = SpriteDirection.Down
        If angle > 180 - (range / 2) AndAlso angle < 180 + (range / 2) Then d = SpriteDirection.Left
        If angle > 270 - (range / 2) AndAlso angle < 270 + (range / 2) Then d = SpriteDirection.Up
        If angle > 360 - (range / 2) AndAlso angle < 0 + (range / 2) Then d = SpriteDirection.Down


        Return d
    End Function

    Public Shared Function GetDirectionFromTrajectory(ByVal trajectory As PointF)
        'Dim mv As New MovementTrajectory(trajectory.X, trajectory.Y)

        'Return mv.Direction

        Return GetDirectionFromAngle(Geometry.GetAngleInDegrees(trajectory))
    End Function

    Public Shared Function GetFrameLettersFromAnimationString(ByVal ani As String) As String
        Dim frames As String() = ani.Split(","c)
        Dim ret As New StringBuilder


        For Each f As String In frames
            ret.Append(f.Split(":"c)(0))

        Next

        Return ret.ToString
    End Function
End Class
