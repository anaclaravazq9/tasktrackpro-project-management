window.chartGantt = null;

window.dibujarGanttConApex = (selector, tareas) => {
    if (window.chartGantt) {
        window.chartGantt.destroy();
        window.chartGantt = null;
    }

    const series = [
        {
            data: tareas.map(t => ({
                x: t.titulo,
                y: [new Date(t.fechaInicio).getTime(), new Date(t.fechaFin).getTime()],
                fillColor: t.estado === 3
                    ? 'rgb(113,216,94)' // verde
                    : t.holgura === 0
                        ? '#FF4560' // rojo si crítica
                        : '#0cd2fd', // azul por defecto
                customData: {
                    titulo: t.titulo,
                    fechaInicio: t.fechaInicio,
                    fechaFinReal: t.fechaFinReal,
                    holgura: t.holgura,
                    estado: t.estado
                }
            }))
        }
    ];

    const options = {
        chart: {
            type: 'rangeBar',
            height: 450
        },
        plotOptions: {
            bar: {
                horizontal: true,
                rangeBarGroupRows: true
            }
        },
        xaxis: {
            type: 'datetime'
        },
        fill: {
            type: 'solid'
        },
        tooltip: {
            enabled: true,
            custom: function({ series, seriesIndex, dataPointIndex, w }) {
                const data = w.config.series[seriesIndex].data[dataPointIndex].customData;
                const formatFecha = (fechaStr) => {
                    const [aaaa, mm, dd] = fechaStr.split('-').map(Number);
                    const fecha = new Date(aaaa, mm - 1, dd);
                    return `${String(fecha.getDate()).padStart(2, '0')}/${String(fecha.getMonth() + 1).padStart(2, '0')}/${fecha.getFullYear()}`;
                };
                return `<div style="padding: 6px;">
                    <strong>${data.titulo}</strong><br/>
                    ${formatFecha(data.fechaInicio)} - ${formatFecha(data.fechaFinReal)}<br/>
                     ${
                    data.estado === 3
                        ? 'Tarea Completada'
                        : `Holgura: ${data.holgura}`
                }
                </div>`;
            }
        }
    };

    const chart = new ApexCharts(document.querySelector(selector), { series, ...options });
    chart.render();
    window.chartGantt = chart;
};