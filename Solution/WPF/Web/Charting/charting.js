function plainBarChart() {
    var data = [
      {
          x: ['giraffes', 'orangutans', 'monkeys'],
          y: [20, 14, 23],
          type: 'bar'
      }
    ];

    Plotly.newPlot('plotly-div', data);
}
function groupedBarChart() {
    var trace1 = {
        x: ['giraffes', 'orangutans', 'monkeys'],
        y: [20, 14, 23],
        name: 'SF Zoo',
        type: 'bar'
    };

    var trace2 = {
        x: ['giraffes', 'orangutans', 'monkeys'],
        y: [12, 18, 29],
        name: 'LA Zoo',
        type: 'bar'
    };

    var data = [trace1, trace2];

    var layout = { barmode: 'group' };

    Plotly.newPlot('plotly-div', data, layout);
}

function stackedBarChart() {
    var trace1 = {
        x: ['giraffes', 'orangutans', 'monkeys'],
        y: [20, 14, 23],
        name: 'SF Zoo',
        type: 'bar'
    };

    var trace2 = {
        x: ['giraffes', 'orangutans', 'monkeys'],
        y: [12, 18, 29],
        name: 'LA Zoo',
        type: 'bar'
    };

    var data = [trace1, trace2];

    var layout = { barmode: 'stack' };

    Plotly.newPlot('plotly-div', data, layout);
}

function barChartWithHoverText() {
    var trace1 = {
        x: ['Product A', 'Product B', 'Product C'],
        y: [20, 14, 23],
        type: 'bar',
        text: ['27% market share', '24% market share', '19% market share'],
        marker: {
            color: 'rgb(158,202,225)',
            opacity: 0.6,
            line: {
                color: 'rbg(8,48,107)',
                width: 1.5
            }
        }
    };

    var data = [trace1];

    var layout = {
        title: 'January 2013 Sales Report'
    };

    Plotly.newPlot('plotly-div', data, layout);
}

function barChartWithDirectLabels(aaa) {

    debugger;

    var asdf = aaa;
    var dfghdfhfg = JSON.parse(aaa);

    var xValue = ['Product A', 'Product B', 'Product C'];

    var yValue = [20, 14, 23];

    var trace1 = {
        x: xValue,
        y: yValue,
        type: 'bar',
        text: ['27% market share', '24% market share', '19% market share'],
        marker: {
            color: 'rgb(158,202,225)',
            opacity: 0.6,
            line: {
                color: 'rbg(8,48,107)',
                width: 1.5
            }
        }
    };

    var annotationContent = [];

    var data = [trace1];

    var layout = {
        title: 'January 2013 Sales Report',
        annotations: annotationContent
    };

    for (var i = 0 ; i < xValue.length ; i++) {
        var result = {
            x: xValue[i],
            y: yValue[i],
            text: yValue[i],
            xanchor: 'center',
            yanchor: 'bottom',
            showarrow: false
        };
        annotationContent.push(result);
    }

    Plotly.newPlot('plotly-div', data, layout);

    var dfgdfgdfgdf = JSON.stringify(layout);

    //alert(layout);
    //alert(JSON.stringify(layout));

    //window.external.Loga(aaa);
    //window.external.Prop1 = "asd";
    window.external.Prop1 = data;
    alert(aaa);
    var sss = JSON.stringify(aaa);
    alert(sss);
}

//plainBarChart();
//groupedBarChart();
//stackedBarChart();
//barChartWithHoverText();
//barChartWithDirectLabels();

