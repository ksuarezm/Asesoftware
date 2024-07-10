Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports System.Data.SqlClient
Imports Newtonsoft.Json.Linq

Public Class APITurnosController
    Inherits ApiController

    Private ReadOnly _connectionString As String = ConfigurationManager.ConnectionStrings("AsesoftwareConnectionString").ConnectionString

    <HttpPost>
    <Route("api/turnos/generar_turnos")>
    Public Function GenerarTurnos(<FromBody> parametros As JObject) As IHttpActionResult

        Try
            If parametros Is Nothing OrElse Not parametros.HasValues Then
                Return BadRequest("Parámetros no válidos.")
            End If

            Dim fechaInicio As Date
            Dim fechaFin As Date
            Dim idServicio As Integer

            Try
                fechaInicio = parametros.GetValue("fechaInicio").ToObject(Of Date)
                fechaFin = parametros.GetValue("fechaFin").ToObject(Of Date)
                idServicio = parametros.GetValue("idServicio").ToObject(Of Integer)
            Catch ex As Exception
                Return BadRequest("Formato de parámetros no válido.")
            End Try

            Dim fechaInicioFormateada As String = fechaInicio.ToString("yyyy-MM-dd")
            Dim fechaFinFormateada As String = fechaFin.ToString("yyyy-MM-dd")

            Dim turnosGenerados As New List(Of Object)

            Using conn As New SqlConnection(_connectionString)
                conn.Open()

                Using cmd As New SqlCommand("MANAGEMENT_TRAERTURNOS_ID", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@FECHAINICIO", fechaInicioFormateada)
                    cmd.Parameters.AddWithValue("@FECHAFIN", fechaFinFormateada)
                    cmd.Parameters.AddWithValue("@ID_SERVICIO", idServicio)

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim turno As New With {
                            .TurnoID = reader.GetInt32(0),
                            .IdServicioTurno = reader.GetInt32(1),
                            .FechaTurno = reader.GetDateTime(2).ToString("dd/MM/yyyy"),
                            .HoraInicio = reader.GetTimeSpan(3).ToString("hh\:mm"),
                            .HoraFin = reader.GetTimeSpan(4).ToString("hh\:mm"),
                            .DescripcionEstado = reader.GetString(6),
                            .IdServicio = reader.GetInt32(7),
                            .IdComercio = reader.GetInt32(8),
                            .NombreServicio = reader.GetString(9),
                            .HoraApertura = reader.GetTimeSpan(10).ToString("hh\:mm"),
                            .HoraCierre = reader.GetTimeSpan(11).ToString("hh\:mm")
                            }
                            turnosGenerados.Add(turno)
                        End While
                    End Using
                End Using
            End Using

            Return Ok(turnosGenerados)
        Catch ex As Exception
            Dim errorResponse As Object = New With {.error = ex.Message}
            Return Me.InternalServerError(New Exception(ex.Message))
        End Try
    End Function
End Class
