import 'dart:math';

import 'package:intl/intl.dart';
import 'package:stockolio/model/exchange/exchange.dart';
import 'package:stockolio/model/market/stock_chart.dart';
import 'package:stockolio/model/market/stock_info.dart';
import 'package:stockolio/model/market/stock_quote.dart';
import 'package:stockolio/model/profile/stock_profile.dart';

import 'company/company_info.dart';
import 'market/stock_overview.dart';

class SampleData {
  static List<StockOverviewModel> getAll() {
    final List<StockOverviewModel> data = [];

    final exchanges = getExchanges();

    for (var comp in listCompany) {
      int rdExchangeIndex = new Random().nextInt(exchanges.length);

      var exchange = exchanges[rdExchangeIndex];

      double priceNum = (11000 + new Random().nextInt(99000)).toDouble();
      double changePercentNum =
          new Random().nextInt(99) + new Random().nextDouble();
      double changeAmount = (-1000 + new Random().nextInt(1500)).toDouble();
      data.add(
        StockOverviewModel(
            code: comp.code!,
            name: comp.name!,
            exchangeCode: exchange.code,
            price: priceNum,
            changeAmount: changeAmount,
            changePercent: changePercentNum),
      );
    }

    return data;
  }

  static List<StockOverviewModel> getWatchList() {
    final List<StockOverviewModel> data = [];

    var sampleData = getAll();

    for (var i = 0; i < 5; i++) {
      data.add(sampleData[i]);
    }

    return data;
  }

  static List<StockOverviewModel> getExchangeSymbols(String exchangeCode) {
    final sampleData = getAll();
    final List<StockOverviewModel> data = sampleData
        .where((element) => element.exchangeCode.contains(exchangeCode))
        .toList();

    return data;
  }

  static List<StockOverviewModel> getExchangeSymbolByCodes(
      List<String> symbolCodes) {
    final sampleData = getAll();
    final List<StockOverviewModel> data = sampleData
        .where((element) => symbolCodes.contains(element.code))
        .toList();

    return data;
  }

  static List<ExchangeModel> getExchanges() {
    return [
      ExchangeModel(code: 'HNX', name: 'HNX', curency: 'VND'),
      ExchangeModel(code: 'HOSE', name: 'HOSE', curency: 'VND'),
      ExchangeModel(code: 'UPCOM', name: 'UPCOM', curency: 'VND'),
      ExchangeModel(code: 'VN30', name: 'VN30', curency: 'VND')
    ];
  }

  static StockProfile getStockProfile(String symbolCode) {
    return new StockProfile(
        stockInfo: new StockInfo(
            ceo: 'Pham Nhat Vuong',
            beta: '',
            changes: 1500,
            changesPercentage: '0.4%',
            companyName: 'Vincom Retail',
            description:
                'C??ng ty Vincom Retail ???????c th??nh l???p ban ?????u v??o ng??y 11/4/2012 d?????i h??nh th???c c??ng ty tr??ch nhi???m h???u h???n. Tr?????c ????, T???p ??o??n Vingroup b???t ?????u ph??t tri???n c??c TTTM th????ng hi???u ???Vincom??? t??? n??m 2004. C??c TTTM n??y g??p ph???n quan tr???ng trong quy ho???ch ph??t tri???n t???ng th??? c??c d??? ??n ph???c h???p v?? khu c??n h??? do T???p ??o??n Vingroup ph??t tri???n. T??? n??m 2013, Vincom Retail ???????c ?????nh h?????ng l?? ????n v??? ph??t tri???n v?? v???n h??nh h??? th???ng TTTM mang th????ng hi???u Vincom c???a T???p ??o??n, ?????ng th???i c??ng ???????c chuy???n th??nh c??ng ty c??? ph???n k??? t??? ng??y 14/5/2013.',
            exchange: 'HSX',
            industry: 'Real Estate',
            mktCap: '2,328,818,410',
            price: 27.5,
            sector: 'VN30',
            volAvg: '2,774,972'),
        stockQuote: new StockQuote(
            avgVolume: 2774972,
            change: 1500,
            changesPercentage: 1.5,
            dayHigh: 28000,
            dayLow: 23000,
            eps: 1.05,
            pe: 26.12,
            marketCap: 2328818410,
            name: 'Vincom Retail',
            open: 23400,
            previousClose: 23400,
            price: 27500,
            sharesOutstanding: 2328818410,
            symbol: 'BTCUSDT',
            volume: 2000000000,
            yearHigh: 40000,
            yearLow: 20000),
        stockCharts: SampleData.listSampleStockChart());
  }

  static List<StockChart> listSampleStockChart() {
    List<StockChart> stockCharts = [];

    var rd = new Random();

    var toDay = DateTime.now();
    var startDate = toDay.add(new Duration(days: -30));

    for (var i = 0; i < 30; i++) {
      var chartDate = startDate.add(new Duration(days: i));

      String formatDate = DateFormat('dd-MM-yyyy').format(chartDate);

      stockCharts.add(new StockChart(
          date: chartDate, close: rd.nextDouble(), label: formatDate));
    }

    return stockCharts;
  }
}

final f = new NumberFormat("#,###", "en_US");
final rateFormat = new NumberFormat("#,###.##", "en_US");

List<CompanyInfo> listCompany = [
  CompanyInfo(code: 'TCB', name: 'Techcombank'),
  CompanyInfo(code: 'VHM', name: 'Vinhomes'),
  CompanyInfo(code: 'HPG', name: 'Hoa Phat'),
  CompanyInfo(code: 'NVL', name: 'Novaland'),
  CompanyInfo(code: 'VPB', name: 'VP Bank'),
  CompanyInfo(code: 'VNM', name: 'Vinamilk'),
  CompanyInfo(code: 'HSG', name: 'Hoa Sen'),
  CompanyInfo(code: 'CTG', name: 'Ngan hang cong thuong viet nam'),
  CompanyInfo(code: 'MSN', name: 'Masan'),
  CompanyInfo(code: 'PNJ', name: 'PNJ'),
  CompanyInfo(code: 'VCB', name: 'Vietcombank'),
  CompanyInfo(code: 'VIC', name: 'Vingroup'),
  CompanyInfo(code: 'SSI', name: 'SSI Stock Company'),
  CompanyInfo(code: 'SCB', name: 'SC Bank'),
  CompanyInfo(code: 'ASM', name: 'Sao mai company'),
  CompanyInfo(code: 'MWG', name: 'The gioi di dong'),
  CompanyInfo(code: 'PLX', name: 'Tap doan xang dau viet nam'),
  CompanyInfo(code: 'SAB', name: 'Sabeco'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
  CompanyInfo(code: 'VRE', name: 'Vincom Retail'),
];
