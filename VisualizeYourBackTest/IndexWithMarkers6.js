
var chart = LightweightCharts.createChart(document.body, {
  width: 1800,
  height: 880,
  timeScale: {
    timeVisible: true,
    borderColor: '#D1D4DC',
  },
  rightPriceScale: {
    visible: true,
    borderColor: '#D1D4DC',
  },
  leftPriceScale: {
    visible: true,
  },
  layout: {
    backgroundColor: '#ffffff',
    textColor: '#000',
  },
  grid: {
    horzLines: {
      color: '#F0F3FA',
    },
    vertLines: {
      color: '#F0F3FA',
    },
  },
  crosshair: {
    mode: 0, // CrosshairMode.Normal
  },
});

var series = chart.addCandlestickSeries({
  upColor: 'rgb(38,166,154)',
  downColor: 'rgb(255,82,82)',
  wickUpColor: 'rgb(38,166,154)',
  wickDownColor: 'rgb(255,82,82)',
  borderVisible: false,
});

const getLocalData = async () => {  
  var res = await fetch('20221208_ES5mMACDLongV2_5.txt'); 
  var resp = await res.text();
  //console.log(resp);
  var cdata = resp.split('\r\n');  
  cdata.pop();
  return cdata;
};

// get candle
const getCandleData = (data) => {
  var cdata = data.map((row) => {
    var [time, open, high, low, close, action, price, profit, totalprofit] = row.split(',');
    return {
      time: (new Date(`${time}`)).setHours((new Date(`${time}`)).getHours() - 6) / 1000, // get NY time
      open: open * 1,
      high: high * 1,
      low: low * 1,
      close: close * 1,
    };
  });
  console.log(cdata[cdata.length - 1]);
  return cdata;
};

// get marker
const getMarkerData = (data) => {
  var cdata = data.map((row) => {
    var [time, open, high, low, close, action, price, profit, totalprofit] = row.split(',');
    return {
      time: (new Date(`${time}`)).setHours((new Date(`${time}`)).getHours() - 6) / 1000,
      position: action == '1' ? 'aboveBar' : (action == '2' ? 'belowBar' : ''),
      color: action == '1' ? '#e91e63' : (action == '2' ? '#2196F3' : ''),
      shape: action == '1' ? 'arrowDown' : (action == '2' ? 'arrowUp' : ''),
      text: action == '1' ? 'S@' + price : (action == '2' ? 'B@' + price : ''),
    };
  });
  var filtered = [];
  cdata.forEach(s => {
    if (s.text != '') {
      filtered.push(s);
    }
  })
  return filtered;
};

// get total profit
const getTotalProfitData = (data) => {
  var cdata = data.map((row) => {
    var [time, open, high, low, close, action, price, profit, totalprofit] = row.split(',');
    return {
      time: (new Date(`${time}`)).setHours((new Date(`${time}`)).getHours() - 6) / 1000,
      value: totalprofit,
    };
  });
  var filtered = [];
  cdata.forEach(s => {
    if (s.value > 0.0) {
      filtered.push(s);
    }
  });
  return filtered;
};

// get buy/sell point
const getEntryData = (data) => {
  var cdata = data.map((row) => {
    var [time, open, high, low, close, action, price, profit, totalprofit] = row.split(',');
    return {
      time: (new Date(`${time}`)).setHours((new Date(`${time}`)).getHours() - 6) / 1000,
      value: price,
    };
  });
  var filtered = [];
  cdata.forEach(s => {
    if (s.value > 0.0) {
      filtered.push(s);
    }
  });
  return filtered;
};

displayChart = async function () {
  //const cdata = await getData();
  const localdata = await getLocalData();

  const data = getCandleData(localdata); // 
  // candels
  series.setData(data);
  console.log(0)
  console.log(data)

  // markers
  const markers = getMarkerData(localdata); // 
  series.setMarkers(markers);
  console.log(1)
  console.log(markers)

  //total profit local
  const totalprofitlocal = getTotalProfitData(localdata);
  const leftSerieslocal = chart.addLineSeries({
    priceScaleId: 'left',
    color: '#334FFF', //blue
  });
  leftSerieslocal.setData(totalprofitlocal);

  // buy/sell points
  const entrypoint = getEntryData(localdata);
  const entrylines = chart.addLineSeries({
    lineWidth: 1,
  });
  entrylines.setData(entrypoint);

};
displayChart();