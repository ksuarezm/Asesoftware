
@Code
    Dim listadoServicios = ViewBag.listadoServicios
End Code

<div class="container">
    <div class="panel panel-default">
        <div class="panel-heading">
            <strong class="text-primary_asalud">TOMA DE TURNOS</strong>
        </div>
    </div>
    <br />

    <div class="panel-default">
        <div class="panel-heading">
            <div class="panel-body">
                <form id="formulario">
                    <div class="row">
                        <div class="col-md-3">
                            <label class="tituloDos">Fecha inicio</label>
                            <input type="date" id="fecha_inicio" name="fecha_inicio" class="form-control" required />
                        </div>
                        <div class="col-md-3">
                            <label class="tituloDos">Fecha fin</label>
                            <input type="date" id="fecha_fin" name="fecha_fin" class="form-control" required />
                        </div>
                        <div class="col-md-3">
                            <label class="tituloDos">Id servicio</label>
                            <select id="id_servicio" name="id_servicio" class="form-control" required>
                                <option value="">-Seleccione-</option>
                                @For Each servicio In listadoServicios
                                    @<option value="@servicio.ID_SERVICIO">@servicio.NOM_SERVICIO</option>
                                Next
                            </select>
                        </div>
                    </div>
                    <div class="row" style="text-align:right;">
                        <div class="col-md-12">
                            <a class="btn boton_aceptar" onclick="EnviarDatos()">Validar</a>
                        </div>
                    </div>
                </form>
            </div>
            <br />


            <div class="panel-body">
                <div class="table-responsive">
                    <table id="tabla" class="table-bordered" style="width:98%;">
                        <thead>
                            <tr>
                                <th>Id turno</th>
                                <th>Fecha turno</th>
                                <th>Hora inicio</th>
                                <th>Hora fin</th>
                                <th>Estado</th>
                                <th>Servicio</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>


<script>

    $(document).ready(function () {

        $('#tabla').DataTable({
            "searching": true,
            "iDisplayLength": 10,
            "lengthChange": false,
            "info": false,
            responsive: true,
            language: {
                processing: "Procesando...",
                search: "Buscar:",
                lengthMenu: "Mostrar MENU registros",
                info: "Mostrando registros del START al END de un total de TOTAL registros",
                infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                infoFiltered: "(filtrado de un total de MAX registros)",
                infoPostFix: "",
                loadingRecords: "Cargando...",
                zeroRecords: "No se encontraron resultados",
                emptyTable: "Ningún dato disponible en esta tabla",
                paginate: {
                    first: "Primero",
                    previous: "Anterior",
                    next: "Siguiente",
                    last: "Último",
                },
            }
        });
    });

    function EnviarDatos() {

        debugger

        var fechaInicio = $("#fecha_inicio").val();
        var fechaFin = $("#fecha_fin").val();
        var idServicio = $("#id_servicio").val();

        if (fechaInicio && fechaFin && idServicio) {
            var laUrl = '@Url.Action("ObtenerTurnosAsync")?fechaInicio=' + fechaInicio + "&fechaFin=" + fechaFin + "&idServicio=" + idServicio;
            $.ajax({
                type: "POST",
                url: laUrl,
                data: {},
                success: function (response) {
                    // Limpiar tabla antes de llenarla
                    $('#tabla tbody').empty();

                    // Llenar tabla con los datos recibidos
                    debugger

                    var parsedResponse = JSON.parse(response);

                    if (parsedResponse.LlenadoTabla && parsedResponse.LlenadoTabla.length > 0) {
                        // Llenar tabla con los datos recibidos
                        $.each(parsedResponse.LlenadoTabla, function (index, turno) {
                            // Construir fila de la tabla
                            var fila = '<tr>' +
                                '<td>' + turno.TurnoID + '</td>' +
                                '<td>' + turno.FechaTurno + '</td>' +
                                '<td>' + turno.HoraInicio + '</td>' +
                                '<td>' + turno.HoraFin + '</td>' +
                                '<td>' + turno.DescripcionEstado + '</td>' +
                                '<td>' + turno.NombreServicio + '</td>' +
                                '</tr>';

                            // Agregar fila a la tabla
                            $('#tabla tbody').append(fila);
                        });
                    } else {
                        // Mostrar mensaje si no hay datos
                        $('#tabla tbody').html('<tr><td colspan="6">No se encontraron turnos</td></tr>');
                    }
                },
                error: function (xhr, status, error) {
                    // Manejar errores si es necesario
                    console.error('Error al obtener los turnos:', error);
                }
            });
        }
        else {
            alert("Complete los campos");
            return false;
        }
    }
</script>




<style>


    table {
        font-family: "Century Gothic", "Century Gothic", Sans-Serif;
        margin: 10px;
        width: 500px;
        text-align: left;
        border-collapse: collapse;
    }

    th {
        font-size: 12px;
        font-weight: bold;
        padding: 8px;
        background: #636363;
        border-top: 4px solid #aabcfe;
        border-bottom: 1px solid #fff;
        color: #e8e8e8;
    }

    td {
        padding: 8px;
        border-bottom: 2px solid #fff;
        color: #636363;
        border-top: 1px solid transparent;
    }

    tr:hover td {
        background: #bcbcbc;
        color: #1c1c1c;
    }
</style>
