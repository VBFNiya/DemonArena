Public Class Geometry
    Public Shared Function CalculateDistance(ByVal pt1 As PointF, ByVal pt2 As PointF) As Double
        Return Math.Sqrt(((pt1.X - pt2.X) ^ 2) + ((pt1.Y - pt2.Y) ^ 2))
    End Function

    Public Shared Function MovePositionAlongTrajectory(ByVal position As PointF, ByVal trajectory As PointF, ByVal distance As Double) As PointF

        Dim baseTraj As PointF = GetBaseTrajectory(trajectory)


        Return New PointF(position.X + (baseTraj.X * distance),
                         position.Y + (baseTraj.Y * distance))
    End Function

    Public Shared Function GetAngleOfTarget(ByVal source As PointF, ByVal target As PointF) As Double

        Dim vector As New PointF(target.X - source.X,
                                 target.Y - source.Y)

        Return GetAngleInDegrees(vector)
    End Function

    Public Shared Function GetAngleInDegrees(ByVal trajectory As PointF) As Double

        '**********************
        'Credit to Jemidiah at VBForums.com
        '**********************

        Dim radians As Double = Math.Atan2(trajectory.Y, trajectory.X)
        Dim degrees As Double = radians * (180 / Math.PI)

        'Anything less than zero means a trajectory above
        'the X axis. -45 would be upper right but I want
        '315 instead so I subtract 45 from 360 via addition since
        'addition of a large positive and small negative is
        'the same as subtracting the absolute of the small value
        'from the big value.
        If degrees < 0 Then
            degrees = 360 + degrees
        End If

        Return degrees
    End Function

    Public Shared Function GetRiseRunFromAngle(ByVal angleDegrees As Double) As PointF

        'Needs to convert back to the angle Atan2 would have
        'produced
        If angleDegrees > 180 Then
            angleDegrees -= 360
        End If

        Dim radians As Double = angleDegrees * (Math.PI / 180)
        Dim tangent As Double = Math.Tan(radians)

        Return New PointF(1, tangent)
    End Function

    Public Shared Function GetTrajectoryFromAngle(ByVal angleDegrees As Double) As PointF
        '**********************
        'Credit to Jemidiah at VBForums.com
        '**********************

        'Needs to convert back to the angle Atan2 would have
        'produced
        If angleDegrees > 180 Then
            angleDegrees -= 360
        End If

        Dim radians As Double = angleDegrees * (Math.PI / 180)
        Dim tangent As Double = Math.Tan(radians)

        Return New PointF(Math.Cos(radians), Math.Sin(radians))
    End Function

    Private Shared Function GetBaseTrajectory(ByVal trajectory As PointF)
        Dim speed As Double = 1

        Dim _x As Double = trajectory.X
        Dim _y As Double = trajectory.Y

        If speed <= 0 Then
            _x = 0
            _y = 0
            Return New PointF(_x, _y)
        End If

        'The sign is important to determine
        'direction along the axis but we must
        'do speed calculations on absolute values
        'So use these variables to preserve the signs
        'after the calculations
        Dim bNegX As Boolean = _x < 0
        Dim bNegY As Boolean = _y < 0

        Dim absX As Double = Math.Abs(_x)
        Dim absY As Double = Math.Abs(_y)

        If absX <> 0 AndAlso absY <> 0 Then
            If absX > absY Then
                absY = (absY / absX) * speed
                absX = speed
            End If

            If absY > absX Then
                absX = (absX / absY) * speed
                absY = speed
            End If

            If absX = absY Then
                absY = speed
                absX = speed
            End If
        Else
            If absX = 0 Then
                absY = speed
            End If
            If absY = 0 Then
                absX = speed
            End If
        End If

        _x = absX
        _y = absY

        If bNegX Then _x = Neg(_x)
        If bNegY Then _y = Neg(_y)

        Return New PointF(_x, _y)
    End Function

    Private Shared Function Neg(ByVal number As Double) As Double
        If number = 0 Then Return 0

        If number < 0 Then
            Return number
        Else
            Return number - (number * 2)
        End If
    End Function



End Class
