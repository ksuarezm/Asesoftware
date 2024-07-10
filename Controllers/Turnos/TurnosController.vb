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
                Dim baseUrl = ""
                Dim currentContext As HttpContextBase = ControllerContext.HttpContext

                If currentContext IsNot Nothing AndAlso currentContext.Request IsNot Nothing AndAlso currentContext.Request.Url IsNot Nothing Then
                    'Obtener el puerto del servidor local - esto si esta desde localhost, que seria este caso, donde seria dinamico

                    Dim port As Integer = currentContext.Request.Url.Port

                    ' Construir la URL de http

                    Dim apiBase As String = $"{currentContext.Request.Url.Scheme}://{currentContext.Request.Url.Authority}/"
                    baseUrl = apiBase
                Else
                    Throw New Exception("Mo se puede obtener direccion HTTP")
                End If


                Using httpClient As New HttpClient()

                    httpClient.Timeout = TimeSpan.FromSeconds(20)
                    Dim data As New Dictionary(Of String, Object)()
                    data.Add("fechaInicio", fechaInicio)
                    data.Add("fechaFin", fechaFin)
                    data.Add("idServicio", idServicio)

                    Dim jsonContent As String = JsonConvert.SerializeObject(data)
                    Dim content As New StringContent(jsonContent, Encoding.UTF8, "application/json")

                    Dim response As HttpResponseMessage = Await httpClient.PostAsync(baseUrl & "api/turnos/generar_turnos", content)

                    If response.IsSuccessStatusCode Then
                        Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                        resultado.LlenadoTabla = JsonConvert.DeserializeObject(Of List(Of Object))(jsonResponse)
                        resultado.conteo = resultado.LlenadoTabla.Count
                        resultado.rta = 1

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

    End Class
End Namespace


