Imports System.Runtime.InteropServices

Public Class GDI32

    ''' <summary>
    '''     Specifies a raster-operation code. These codes define how the color data for the
    '''     source rectangle is to be combined with the color data for the destination
    '''     rectangle to achieve the final color.
    ''' </summary>
    Enum TernaryRasterOperations As UInteger
        ''' <summary>dest = source</summary>
        SRCCOPY = &HCC0020
        ''' <summary>dest = source OR dest</summary>
        SRCPAINT = &HEE0086
        ''' <summary>dest = source AND dest</summary>
        SRCAND = &H8800C6
        ''' <summary>dest = source XOR dest</summary>
        SRCINVERT = &H660046
        ''' <summary>dest = source AND (NOT dest)</summary>
        SRCERASE = &H440328
        ''' <summary>dest = (NOT source)</summary>
        NOTSRCCOPY = &H330008
        ''' <summary>dest = (NOT src) AND (NOT dest)</summary>
        NOTSRCERASE = &H1100A6
        ''' <summary>dest = (source AND pattern)</summary>
        MERGECOPY = &HC000CA
        ''' <summary>dest = (NOT source) OR dest</summary>
        MERGEPAINT = &HBB0226
        ''' <summary>dest = pattern</summary>
        PATCOPY = &HF00021
        ''' <summary>dest = DPSnoo</summary>
        PATPAINT = &HFB0A09
        ''' <summary>dest = pattern XOR dest</summary>
        PATINVERT = &H5A0049
        ''' <summary>dest = (NOT dest)</summary>
        DSTINVERT = &H550009
        ''' <summary>dest = BLACK</summary>
        BLACKNESS = &H42
        ''' <summary>dest = WHITE</summary>
        WHITENESS = &HFF0062
        ''' <summary>
        ''' Capture window as seen on screen.  This includes layered windows
        ''' such as WPF windows with AllowsTransparency="true"
        ''' </summary>
        CAPTUREBLT = &H40000000
    End Enum

    '
    ' currently defined blend operation
    '
    Const AC_SRC_OVER As Integer = &H0

    '
    ' currently defined alpha format
    '
    Const AC_SRC_ALPHA As Byte = &H1

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure BLENDFUNCTION
        Private BlendOp As Byte
        Private BlendFlags As Byte
        Private SourceConstantAlpha As Byte
        Private AlphaFormat As Byte

        Public Sub New(ByVal alpha As Byte)
            Me.New(AC_SRC_OVER, 0, alpha, AC_SRC_ALPHA)
        End Sub

        Public Sub New(ByVal op As Byte, ByVal flags As Byte, ByVal alpha As Byte, ByVal format As Byte)
            BlendOp = op
            BlendFlags = flags
            SourceConstantAlpha = alpha
            AlphaFormat = format
        End Sub
    End Structure

    <DllImport("gdi32.dll", EntryPoint:="GdiAlphaBlend")> _
    Public Shared Function AlphaBlend(ByVal hdcDest As IntPtr, ByVal nXOriginDest As Integer, ByVal nYOriginDest As Integer, ByVal nWidthDest As Integer, ByVal nHeightDest As Integer, ByVal hdcSrc As IntPtr, _
                                      ByVal nXOriginSrc As Integer, ByVal nYOriginSrc As Integer, ByVal nWidthSrc As Integer, ByVal nHeightSrc As Integer, ByVal blendFunction As BLENDFUNCTION) As Boolean
    End Function


    <DllImport("gdi32.dll")> _
    Public Shared Function StretchBlt(ByVal hdcDest As IntPtr, ByVal nXOriginDest As Integer, ByVal nYOriginDest As Integer, ByVal nWidthDest As Integer, ByVal nHeightDest As Integer, ByVal hdcSrc As IntPtr, _
                                      ByVal nXOriginSrc As Integer, ByVal nYOriginSrc As Integer, ByVal nWidthSrc As Integer, ByVal nHeightSrc As Integer, ByVal dwRop As TernaryRasterOperations) As Boolean
    End Function

    <DllImport("Gdi32.dll")> _
    Public Shared Function CreateCompatibleDC(ByVal hdc As IntPtr) As IntPtr

    End Function

    <DllImport("Gdi32.dll")> _
    Public Shared Function SelectObject(ByVal hdc As IntPtr, ByVal hObject As IntPtr) As IntPtr
    End Function

    <DllImport("gdi32.dll")> _
    Public Shared Function DeleteDC(ByVal hdc As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("gdi32.dll")> _
    Public Shared Function DeleteObject(ByVal hObject As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll")> _
    Public Shared Function GetDC(ByVal hwnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Public Shared Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDc As IntPtr) As Integer
    End Function

    <DllImport("gdi32.dll")> _
    Public Shared Function CreateCompatibleBitmap(ByVal hdc As IntPtr, ByVal nWidth As Integer, ByVal nHeight As Integer) As IntPtr
    End Function

End Class

'TODO: Implementation of other GDI operations
Public Class ManagedGDI

    'Public Shared Sub AlphaBlend(ByVal srcDC As ManagedDc,ByVal destDC As ManagedDc,ByVal 

    Public Shared Sub AlphaBlend(ByVal srcImage As Bitmap, ByVal dest As Graphics, ByVal destRect As Rectangle, ByVal alpha As Byte)
        Dim hBmp As ManagedhBitmap = New ManagedhBitmap(srcImage)
        AlphaBlend(hBmp, dest, destRect, alpha)

        hBmp.Dispose()
    End Sub

    Public Shared Sub AlphaBlend(ByVal srcImage As Bitmap, ByVal dest As ManagedDc, ByVal destRect As Rectangle, ByVal alpha As Byte)
        Dim hBmp As New ManagedhBitmap(srcImage)

        AlphaBlend(hBmp, dest, destRect, alpha)

        hBmp.Dispose()
    End Sub

    Public Shared Sub AlphaBlend(ByVal srcImage As ManagedhBitmap, ByVal dest As Graphics, ByVal destRect As Rectangle, ByVal alpha As Byte)
        Dim dc As New ManagedDc(dest)

        AlphaBlend(srcImage, dc, destRect, alpha)

        dc.Dispose()
    End Sub

    Public Shared Sub AlphaBlend(ByVal srcImage As ManagedhBitmap, ByVal dest As ManagedDc, ByVal destRect As Rectangle, ByVal alpha As Byte)

        Dim hBmp As IntPtr = srcImage.Handle

        Dim destDc As IntPtr = dest.hDc

        Dim oldObj As IntPtr
        Dim memDC As IntPtr = GDI32.CreateCompatibleDC(destDc)
        Dim bool As Boolean

        oldObj = GDI32.SelectObject(memDC, hBmp)

        With destRect
            bool = GDI32.AlphaBlend(destDc, .X, .Y, .Width, .Height, memDC, 0, 0, srcImage.Width, srcImage.Height, New GDI32.BLENDFUNCTION(alpha))
        End With

        'Place stock object back into memory DC
        GDI32.SelectObject(memDC, oldObj)

        GDI32.DeleteDC(memDC)
    End Sub

    Public Class ManagedhBitmap
        Implements IDisposable

        Private _hBitmap As IntPtr
        Private _width As Integer
        Private _height As Integer

        Public Sub New(ByVal gdiPlusBitmap As Bitmap)

            gdiPlusBitmap = gdiPlusBitmap.Clone

            'Black background will allow AlphaBlend to
            'work with images that have transparent backgrounds.
            'Don't ask me why though. - Niya
            _hBitmap = gdiPlusBitmap.GetHbitmap(Color.Black)
            _width = gdiPlusBitmap.Width
            _height = gdiPlusBitmap.Height

            gdiPlusBitmap.Dispose()
        End Sub

        Public ReadOnly Property Width As Integer
            Get
                Return _width
            End Get
        End Property

        Public ReadOnly Property Height As Integer
            Get
                Return _height
            End Get
        End Property

        Public ReadOnly Property Handle As IntPtr
            Get
                Return _hBitmap
            End Get
        End Property

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                GDI32.DeleteObject(_hBitmap)
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        Protected Overrides Sub Finalize()
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(False)
            MyBase.Finalize()
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    Public Class ManagedDc
        Implements IDisposable

        Private Enum DCType
            Control
            Graphics
            Memory
        End Enum

        Private _hDc As IntPtr
        Private _hWnd As IntPtr
        Private _graphics As Graphics

        'Created when a memory DC is created
        Private _hBitmap As IntPtr

        Private g_enType As DCType

        Public Sub New(ByVal control As Control)
            Me.New(control, False)
        End Sub

        Public Sub New(ByVal g As Graphics)
            _graphics = g

            _hDc = g.GetHdc()

            g_enType = DCType.Graphics
        End Sub

        Public Sub New(ByVal control As Control, ByVal createCompatibleDC As Boolean)
            If Not createCompatibleDC Then
                _hDc = GDI32.GetDC(control.Handle)
                _hWnd = control.Handle

                g_enType = DCType.Control
            Else
                'Get the control's DC
                Dim controlDc As IntPtr = GDI32.GetDC(control.Handle)
                Dim width As Integer = control.Width
                Dim height As Integer = control.Height

                'CreateCompatibleBitmap will return a monochrome
                'bitmap if the width and height parameters are 0
                'We want to avoid that
                If width <= 0 Then width = 1
                If height <= 0 Then height = 1

                'Create a memory DC
                _hDc = GDI32.CreateCompatibleDC(controlDc)

                'Create a bitmap from the control's DC so we
                'get a colour bitmap and and not a monochrome one
                _hBitmap = GDI32.CreateCompatibleBitmap(controlDc, width, height)

                'Select the bitmap into the memory DC
                GDI32.SelectObject(_hDc, _hBitmap)

                'Release the control's DC
                GDI32.ReleaseDC(control.Handle, controlDc)

                g_enType = DCType.Memory
            End If
        End Sub

        Public Sub SetSize(ByVal width As Integer, ByVal height As Integer)
            If g_enType <> DCType.Memory Then
                Throw New Exception("This operation is only allowed on memory DCs")
            End If

            Dim hBmpNew As IntPtr = GDI32.CreateCompatibleBitmap(_hDc, width, height)
            Dim hBmpOld As IntPtr = GDI32.SelectObject(_hDc, hBmpNew)

            _hBitmap = hBmpNew

            GDI32.DeleteObject(hBmpOld)
        End Sub

        Public Function GetGDIPlusBitmap() As Bitmap
            If g_enType = DCType.Memory Then
                Return Bitmap.FromHbitmap(_hBitmap)
            Else
                Throw New NotImplementedException("Getting a bitmap from a non-memory DC has not yet been implemented")
            End If
        End Function

        Public ReadOnly Property hDc As IntPtr
            Get
                Return _hDc
            End Get
        End Property


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If
                Select Case g_enType
                    Case DCType.Control
                        GDI32.ReleaseDC(_hWnd, _hDc)
                    Case DCType.Memory
                        GDI32.DeleteObject(_hBitmap)
                        GDI32.DeleteDC(_hDc)
                    Case DCType.Graphics
                        _graphics.ReleaseHdc(_hDc)
                End Select


                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        Protected Overrides Sub Finalize()
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(False)
            MyBase.Finalize()
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class



End Class

