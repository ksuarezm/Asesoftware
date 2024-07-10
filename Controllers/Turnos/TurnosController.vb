Imports System.Data.SqlClient
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Web.Mvc
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace Controllers.Turnos
    Public Class TurnosController
        Inherits Controller

        Private ReadOnly _connectionString As String = ConfigurationManager.ConnectionStrings("AsesoftwareConnectionString").ConnectionString

        ' GET: Turnos
        Function TomarTurnos() As ActionResult


            Dim listadoServicios As New List(Of Object)

            Using conn As New SqlConnection(_connectionString)
                conn.Open()

                Using cmd As New SqlCommand("SELECT * FROM SERVICIOS", conn)
                    cmd.CommandType = CommandType.Text

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim servicio As New With {
                                Key .ID_SERVICIO = reader.GetInt32(0),
                                Key .NOM_SERVICIO = reader.GetString(2)
                            }
                            listadoServicios.Add(servicio)
                        End While
                    End Using

                End Using
            End Using

            ViewBag.listadoServicios = listadoServicios

            Return View()
        End Function

        Public Async Function ObtenerTurnosAsync(fechaInicio As Date?, fechaFin As Date?, idServicio As Integer?) As Task(Of Object)

            Dim resultado As New With {
            .LlenadoTabla = New List(Of Object)(),
            .mensaje = "",
            .rta = Nothing,
            .conteo = 0
            }

            Try
                Dim apiBaseUrl As String = "https://localhost:44398/"

                Using httpClient As New HttpClient()

                    httpClient.Timeout = TimeSpan.FromSeconds(20)
                    Dim data As New Dictionary(Of String, Object)()
                    data.Add("fechaInicio", fechaInicio)
                    data.Add("fechaFin", fechaFin)
                    data.Add("idServicio", idServicio)

                    Dim jsonContent As String = JsonConvert.SerializeObject(data)
                    Dim content As New StringContent(jsonContent, Encoding.UTF8, "application/json")

                    Dim response As HttpResponseMessage = Await httpClient.PostAsync(apiBaseUrl & "api/turnos/generar_turnos", content)

                    If response.IsSuccessStatusCode Then
                        Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                        resultado.LlenadoTabla = JsonConvert.DeserializeObject(Of List(Of Object))(jsonResponse)
                        resultado.conteo = resultado.LlenadoTabla.Count
                    Else
                        Dim errorMessage As String
                        If response.StatusCode = 500 Then
                            errorMessage = "No se encontró el servicio"
                        Else
                            errorMessage = $"Error al obtener los turnos. StatusCode: {response.StatusCode}"
                        End If

                        Throw New Exception(errorMessage)
                    End If
                End Using
            Catch ex As Exception
                resultado.mensaje = ex.Message
                resultado.rta = Nothing
            End Try

            Return JsonConvert.SerializeObject(resultado)

        End Function



        Public Class Turno
            Public Property TurnoID As Integer
            Public Property Fecha As DateTime
            Public Property HoraInicio As String
            Public Property HoraFin As String
        End Class

    End Class
End Namespace