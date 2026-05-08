const palette = ['#E8521A', '#FF8C00', '#2196F3', '#4CAF50', '#9C27B0', '#00BCD4'];
let lineChart, barChart, radarChart, termChart;
let chartData = null;

$.getJSON(`/StudentProgressVisualization/GetChartData?studentId=${studentId}`, function (data) {
    chartData = data;

    

    lineChart = new Chart(document.getElementById('lineChart'), {
        type: 'line',
        data: buildLineData(1),
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: { y: { min: 0, max: 10 } },
            plugins: { legend: { position: 'bottom' } }
        }
    });

    barChart = new Chart(document.getElementById('barChart'), {
        type: 'bar',
        data: {
            labels: data.subjectAverages.map(s => s.subject),
            datasets: [{
                label: 'Avg Score',
                data: data.subjectAverages.map(s => s.avg),
                backgroundColor: palette,
                borderRadius: 8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: { y: { min: 0, max: 10 } },
            plugins: { legend: { display: false } }
        }
    });

    
    radarChart = new Chart(document.getElementById('radarChart'), {
        type: 'radar',
        data: {
            labels: data.subjects,
            datasets: [
                {
                    label: 'This Student',
                    data: data.subjectAverages.map(s => s.avg),
                    borderColor: '#E8521A',
                    backgroundColor: '#E8521A33',
                    pointBackgroundColor: '#E8521A'
                },
                {
                    label: 'Grade Avg',
                    data: data.classAverages.map(s => s.avg),
                    borderColor: '#2196F3',
                    backgroundColor: '#2196F333',
                    pointBackgroundColor: '#2196F3'
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: { r: { min: 0, max: 10 } }
        }
    });

    
    const terms = [1, 2, 3, 4];
    termChart = new Chart(document.getElementById('termChart'), {
        type: 'bar',
        data: {
            labels: terms.map(t => 'Term ' + t),
            datasets: data.subjects.map((s, i) => ({
                label: s,
                data: terms.map(t => {
                    const r = data.termData.find(d => d.subject === s && d.term === t);
                    return r ? r.avg : 0;
                }),
                backgroundColor: palette[i],
                borderRadius: 6
            }))
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: { y: { min: 0, max: 10 } },
            plugins: { legend: { position: 'bottom' } }
        }
    });
});

function buildLineData(term) {
    const filtered = chartData.weeklyData.filter(d => d.term === term);// filter the data according to the selected term
    const weeks = [...new Set(filtered.map(d => d.week))].sort((a, b) => a - b);   //new set removes duplicates, then we sort the weeks in ascending order
    return {
        labels: weeks.map(w => 'Week ' + w),// label for x-axis
        datasets: chartData.subjects.map((s, i) => ({   // loop through subjects to create a dataset for each subject
            label: s,
            data: weeks.map(w => {  // makes the data points for the line chart by finding the average score for each week and for each subject
                const r = filtered.find(d => d.subject === s && d.week === w);
                return r ? r.avg : null;
            }),
            borderColor: palette[i],
            backgroundColor: palette[i] + '22',
            tension: 0.4,
            pointRadius: 4,
            spanGaps: true,
            fill: false
        }))
    };
}

function updateLineChart(term, btn) {
    document.querySelectorAll('.sp-term-btn').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    lineChart.data = buildLineData(term);
    lineChart.update();
}