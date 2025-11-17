Public Class Levels
    Private Shared gshr_lstLevels As New List(Of Level)

    Shared Sub New()

        With gshr_lstLevels
            .Add(New Level(New Type() {GetType(Spirit)}))

            .Add(New Level(New Type() {GetType(Spirit), GetType(Spirit)}))

            .Add(New Level(New Type() {GetType(Spirit), GetType(Spirit), GetType(Spirit)}))

            .Add(New Level(New Type() {GetType(Spirit),
                                       GetType(Spirit),
                                       GetType(Spirit),
                                       GetType(Spirit),
                                       GetType(Spirit)}))


            '.Add(New Level(New Type() {GetType(PurpleAfritSimple)}))

            '.Add(New Level(New Type() {GetType(GoldenAfritSimple)}))

            '.Add(New Level(New Type() {GetType(PurpleAfritSimple),
            'GetType(Spirit),
            'GetType(Spirit)}))


        End With

    End Sub

    Public Shared ReadOnly Property NumberOfLevels As Integer
        Get
            Return gshr_lstLevels.Count
        End Get
    End Property

    Public Shared Function GetLevel(ByVal levelNumber As Integer)
        Return gshr_lstLevels.Item(levelNumber - 1)
    End Function

End Class

Public Class Level
    Private g_lstCreatureTypes As New List(Of Type)

    Public Sub New(ByVal creatureTypes As IEnumerable(Of Creature))
        Me.AddRange(creatureTypes)
    End Sub

    Public Sub Add(ByVal creatureType As Type)
        If creatureType.IsSubclassOf(GetType(Creature)) AndAlso Not creatureType.IsAbstract Then
            g_lstCreatureTypes.Add(creatureType)
        Else
            Throw New Exception("Type must inherit from Creature and must not be abstract")
        End If
    End Sub

    Public Sub AddRange(ByVal creatureTypes As IEnumerable(Of Type))
        For Each t As Type In creatureTypes
            Me.Add(creatureTypes)
        Next
    End Sub

    Public Function CreateInstances() As IList(Of Creature)
        Dim lst As New List(Of Creature)
        For Each t As Type In g_lstCreatureTypes
            lst.Add(DirectCast(Activator.CreateInstance(t), Creature))
        Next
        Return lst.ToArray
    End Function
End Class
