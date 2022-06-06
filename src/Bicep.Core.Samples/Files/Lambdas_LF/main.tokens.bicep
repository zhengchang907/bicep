var doggos = [
//@[000:003) Identifier |var|
//@[004:010) Identifier |doggos|
//@[011:012) Assignment |=|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
  'Evie'
//@[002:008) StringComplete |'Evie'|
//@[008:009) NewLine |\n|
  'Casper'
//@[002:010) StringComplete |'Casper'|
//@[010:011) NewLine |\n|
  'Indy'
//@[002:008) StringComplete |'Indy'|
//@[008:009) NewLine |\n|
  'Kira'
//@[002:008) StringComplete |'Kira'|
//@[008:009) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

var numbers = range(0, 4)
//@[000:003) Identifier |var|
//@[004:011) Identifier |numbers|
//@[012:013) Assignment |=|
//@[014:019) Identifier |range|
//@[019:020) LeftParen |(|
//@[020:021) Integer |0|
//@[021:022) Comma |,|
//@[023:024) Integer |4|
//@[024:025) RightParen |)|
//@[025:027) NewLine |\n\n|

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[000:003) Identifier |var|
//@[004:012) Identifier |sayHello|
//@[013:014) Assignment |=|
//@[015:018) Identifier |map|
//@[018:019) LeftParen |(|
//@[019:025) Identifier |doggos|
//@[025:026) Comma |,|
//@[027:028) Identifier |i|
//@[029:031) Arrow |=>|
//@[032:041) StringLeftPiece |'Hello ${|
//@[041:042) Identifier |i|
//@[042:045) StringRightPiece |}!'|
//@[045:046) RightParen |)|
//@[046:048) NewLine |\n\n|

var isEven = filter(numbers, i => 0 == i % 2)
//@[000:003) Identifier |var|
//@[004:010) Identifier |isEven|
//@[011:012) Assignment |=|
//@[013:019) Identifier |filter|
//@[019:020) LeftParen |(|
//@[020:027) Identifier |numbers|
//@[027:028) Comma |,|
//@[029:030) Identifier |i|
//@[031:033) Arrow |=>|
//@[034:035) Integer |0|
//@[036:038) Equals |==|
//@[039:040) Identifier |i|
//@[041:042) Modulo |%|
//@[043:044) Integer |2|
//@[044:045) RightParen |)|
//@[045:047) NewLine |\n\n|

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[000:003) Identifier |var|
//@[004:027) Identifier |evenDoggosNestedLambdas|
//@[028:029) Assignment |=|
//@[030:033) Identifier |map|
//@[033:034) LeftParen |(|
//@[034:040) Identifier |filter|
//@[040:041) LeftParen |(|
//@[041:048) Identifier |numbers|
//@[048:049) Comma |,|
//@[050:051) Identifier |i|
//@[052:054) Arrow |=>|
//@[055:063) Identifier |contains|
//@[063:064) LeftParen |(|
//@[064:070) Identifier |filter|
//@[070:071) LeftParen |(|
//@[071:078) Identifier |numbers|
//@[078:079) Comma |,|
//@[080:081) Identifier |j|
//@[082:084) Arrow |=>|
//@[085:086) Integer |0|
//@[087:089) Equals |==|
//@[090:091) Identifier |j|
//@[092:093) Modulo |%|
//@[094:095) Integer |2|
//@[095:096) RightParen |)|
//@[096:097) Comma |,|
//@[098:099) Identifier |i|
//@[099:100) RightParen |)|
//@[100:101) RightParen |)|
//@[101:102) Comma |,|
//@[103:104) Identifier |x|
//@[105:107) Arrow |=>|
//@[108:114) Identifier |doggos|
//@[114:115) LeftSquare |[|
//@[115:116) Identifier |x|
//@[116:117) RightSquare |]|
//@[117:118) RightParen |)|
//@[118:119) NewLine |\n|

//@[000:000) EndOfFile ||
