'Implement this on classes where you want O(1) removals
'The FastList class would place a LinkNode reference into
'the LinkReference property so it doesn't have to search for it
Public Interface IFastListItem
    Property LinkReference As Object
End Interface

<DebuggerDisplayAttribute("Count = {Count}")>
Public Class FastList(Of T)
    Implements IList(Of T)

    Private _count As Integer
    Private _firstNode As LinkNode(Of T)
    Private _lastNode As LinkNode(Of T)

    Private g_lstNodeInfo As New List(Of LinkNodeInfo)

    Private Delegate Function FindLinkCondition(ByVal index As Integer, ByVal link As LinkNode(Of T)) As Boolean

    Public Sub New(ByVal collection As IEnumerable(Of T))
        Me.AddRange(collection)
    End Sub

    Public Sub New()

    End Sub

    Public Sub AddRange(ByVal collection As IEnumerable(Of T))
        For Each itm In collection
            Me.Add(itm)
        Next
    End Sub

    Public Sub Add(item As T) Implements System.Collections.Generic.ICollection(Of T).Add
        Insert(_count, item)
    End Sub

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of T).Clear
        CycleNodes(Sub(index, node)
                       If TypeOf node Is IFastListItem Then
                           DirectCast(node, IFastListItem).LinkReference = Nothing
                       End If
                   End Sub)

        _firstNode = Nothing
        _lastNode = Nothing
        _count = 0


        g_lstNodeInfo.Clear()
    End Sub

    Public Function Contains(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
        Return Me.IndexOf(item) <> -1
    End Function

    Public Sub CopyTo(array() As T, arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
        Throw New NotImplementedException()
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of T).Count
        Get
            Return _count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of T).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
        Return New FastListEnumerator(Of T)(_firstNode)
    End Function

    Public Function IndexOf(item As T) As Integer Implements System.Collections.Generic.IList(Of T).IndexOf
        Dim i As Integer = 0
        For Each listItem As T In Me
            If item.Equals(listItem) Then
                Return i
            End If
            i += 1
        Next

        Return -1
    End Function

    Public Sub Insert(index As Integer, item As T) Implements System.Collections.Generic.IList(Of T).Insert
        If index > _count Then
            Throw New IndexOutOfRangeException
        End If

        If index = 0 AndAlso _count = 0 Then
            _firstNode = New LinkNode(Of T)(item, Nothing, Nothing)
            _lastNode = _firstNode

            g_lstNodeInfo.Add(New LinkNodeInfo(_firstNode, index))

            If TypeOf item Is IFastListItem Then
                DirectCast(item, IFastListItem).LinkReference = _firstNode
            End If

        ElseIf index = _count Then
            Dim newLink = New LinkNode(Of T)(item, _lastNode, Nothing)
            _lastNode.NextLink = newLink
            _lastNode = newLink

            g_lstNodeInfo.Add(New LinkNodeInfo(newLink, index))

            If TypeOf item Is IFastListItem Then
                DirectCast(item, IFastListItem).LinkReference = newLink
            End If


        Else
            Dim node = FindNode(Function(idx, cnode) idx = index)
            Dim prevLink = node.PrevLink

            If prevLink IsNot Nothing Then
                prevLink.NextLink = New LinkNode(Of T)(item, prevLink, node)

                g_lstNodeInfo.Add(New LinkNodeInfo(prevLink.NextLink, index))

                If TypeOf item Is IFastListItem Then
                    DirectCast(item, IFastListItem).LinkReference = prevLink.NextLink
                End If

            Else
                'This has to be the first node---
                _firstNode = New LinkNode(Of T)(item, Nothing, node)

                If TypeOf item Is IFastListItem Then
                    DirectCast(item, IFastListItem).LinkReference = _firstNode
                End If
            End If

        End If
        _count += 1
    End Sub

    Default Public Property Item(index As Integer) As T Implements System.Collections.Generic.IList(Of T).Item
        Get
            If index >= _count Then
                Throw New IndexOutOfRangeException
            End If

            Dim i As Integer = 0
            For Each Itm In Me
                If i = index Then
                    Return Itm
                End If
                i += 1
            Next

        End Get
        Set(value As T)
            If index >= _count Then
                Throw New IndexOutOfRangeException
            End If

            Dim node = FindNode(Function(idx, cnode) idx = index)
            node.Item = value

            If TypeOf value Is IFastListItem Then
                DirectCast(value, IFastListItem).LinkReference = node
            End If
        End Set
    End Property

    Public Function Remove(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove

        If item Is Nothing OrElse _firstNode Is Nothing Then Return False
        Dim node As LinkNode(Of T) = Nothing

        'For Each ni As LinkNodeInfo In g_lstNodeInfo
        '    If ni.Node.Item.Equals(item) Then
        '        node = ni.Node

        '        ni.Node = Nothing
        '        ni.Index = -1
        '        Exit For
        '    End If
        'Next

        'If we can avoid searching then removals would be O(1)
        If TypeOf item Is IFastListItem Then
            Dim casted = DirectCast(item, IFastListItem)

            If casted.LinkReference IsNot Nothing Then
                If TypeOf casted.LinkReference Is LinkNode(Of T) Then
                    node = casted.LinkReference
                    casted.LinkReference = Nothing
                Else
                    Throw New InvalidOperationException("LinkReference should only be set by the FastList")
                End If
            Else
                'If LinkReference is Nothing then that means it wasn't
                'added to the list in the first place
                Return False
            End If
        End If

        If node Is Nothing Then
            node = FindNode(Function(index, cnode) cnode.Item.Equals(item))
        End If

        If node IsNot Nothing Then

            Dim p = node.PrevLink
            Dim n = node.NextLink

            'This is the first node
            If p Is Nothing Then
                _firstNode = n
            End If

            If n Is Nothing Then
                _lastNode = p
            End If

            If p IsNot Nothing Then p.NextLink = n
            If n IsNot Nothing Then n.PrevLink = p

            _count -= 1

            Return True

        End If

        Return False

    End Function

    Public Sub RemoveAt(index As Integer) Implements System.Collections.Generic.IList(Of T).RemoveAt
        If index >= _count Then
            Throw New IndexOutOfRangeException
        End If

        Dim node = FindNode(Function(idx, cnode) idx = index)

        If TypeOf node.Item Is IFastListItem Then DirectCast(node.Item, IFastListItem).LinkReference = Nothing

        Dim p = node.PrevLink
        Dim n = node.NextLink

        'This is the first node
        If p Is Nothing Then
            _firstNode = n
        End If

        If n Is Nothing Then
            _lastNode = p
        End If

        If p IsNot Nothing Then p.NextLink = n
        If n IsNot Nothing Then n.PrevLink = p

        _count -= 1


        ''Dim curNode As LinkNode(Of T) = _firstNode
        ''Dim i As Integer = 0

        'Do Until curNode Is Nothing
        '    If i = index Then
        '        Dim p = curNode.PrevLink
        '        Dim n = curNode.NextLink

        '        'This is the first node
        '        If p Is Nothing Then
        '            _firstNode = n
        '        End If

        '        If n Is Nothing Then
        '            _lastNode = p
        '        End If

        '        If p IsNot Nothing Then p.NextLink = n
        '        If n IsNot Nothing Then n.PrevLink = p

        '        _count -= 1
        '        Exit Do
        '    End If

        '    curNode = curNode.NextLink
        '    i += 1
        'Loop



    End Sub

    Private Sub CycleNodes(ByVal exec As Action(Of Integer, LinkNode(Of T)))
        Dim curNode As LinkNode(Of T) = _firstNode
        Dim i As Integer = 0

        Do Until curNode Is Nothing
            exec(i, curNode)

            curNode = curNode.NextLink
            i += 1
        Loop

    End Sub

    Private Function FindNode(ByVal condition As FindLinkCondition) As LinkNode(Of T)

        Dim curNode As LinkNode(Of T) = _firstNode
        Dim i As Integer = 0

        Do Until curNode Is Nothing

            If condition(i, curNode) Then
                Return curNode
            End If

            curNode = curNode.NextLink
            i += 1
        Loop

        Return Nothing
    End Function

    Private Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return Me.GetEnumerator
    End Function

    Private Class LinkNodeInfo
        Public Sub New(node As LinkNode(Of T), ByVal index As Integer)
            Me.Node = node
            Me.Index = index
        End Sub

        Public Node As LinkNode(Of T)
        Public Index As Integer
    End Class

    Private Class LinkNode(Of T1)
        Public Sub New()

        End Sub

        Public Sub New(ByVal item As T1, ByVal prev As LinkNode(Of T1), ByVal [next] As LinkNode(Of T1))
            Me.Item = item
            Me.NextLink = [next]
            Me.PrevLink = prev
        End Sub

        Public Item As T1
        Public NextLink As LinkNode(Of T1)
        Public PrevLink As LinkNode(Of T1)
    End Class


    Private Class FastListEnumerator(Of T2)
        Implements IEnumerator(Of T2)

        Private _firstNode As LinkNode(Of T2)
        Private _curNode As LinkNode(Of T2)

        Public Sub New(ByVal firstNode As LinkNode(Of T2))
            _firstNode = firstNode
            _curNode = Nothing
        End Sub

        Public ReadOnly Property Current As T2 Implements System.Collections.Generic.IEnumerator(Of T2).Current
            Get
                Return _curNode.Item
            End Get
        End Property

        Public ReadOnly Property Current1 As Object Implements System.Collections.IEnumerator.Current
            Get
                Return Me.Current
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
            If _curNode Is Nothing Then
                _curNode = _firstNode
            Else
                _curNode = _curNode.NextLink
            End If

            Return _curNode IsNot Nothing
        End Function

        Public Sub Reset() Implements System.Collections.IEnumerator.Reset
            _curNode = Nothing
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Class


