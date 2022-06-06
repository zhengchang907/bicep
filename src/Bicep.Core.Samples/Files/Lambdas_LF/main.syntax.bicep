var doggos = [
//@[000:297) ProgramSyntax
//@[000:054) ├─VariableDeclarationSyntax
//@[000:003) | ├─Token(Identifier) |var|
//@[004:010) | ├─IdentifierSyntax
//@[004:010) | | └─Token(Identifier) |doggos|
//@[011:012) | ├─Token(Assignment) |=|
//@[013:054) | └─ArraySyntax
//@[013:014) | | ├─Token(LeftSquare) |[|
//@[014:015) | | ├─Token(NewLine) |\n|
  'Evie'
//@[002:008) | | ├─ArrayItemSyntax
//@[002:008) | | | └─StringSyntax
//@[002:008) | | | | └─Token(StringComplete) |'Evie'|
//@[008:009) | | ├─Token(NewLine) |\n|
  'Casper'
//@[002:010) | | ├─ArrayItemSyntax
//@[002:010) | | | └─StringSyntax
//@[002:010) | | | | └─Token(StringComplete) |'Casper'|
//@[010:011) | | ├─Token(NewLine) |\n|
  'Indy'
//@[002:008) | | ├─ArrayItemSyntax
//@[002:008) | | | └─StringSyntax
//@[002:008) | | | | └─Token(StringComplete) |'Indy'|
//@[008:009) | | ├─Token(NewLine) |\n|
  'Kira'
//@[002:008) | | ├─ArrayItemSyntax
//@[002:008) | | | └─StringSyntax
//@[002:008) | | | | └─Token(StringComplete) |'Kira'|
//@[008:009) | | ├─Token(NewLine) |\n|
]
//@[000:001) | | └─Token(RightSquare) |]|
//@[001:003) ├─Token(NewLine) |\n\n|

var numbers = range(0, 4)
//@[000:025) ├─VariableDeclarationSyntax
//@[000:003) | ├─Token(Identifier) |var|
//@[004:011) | ├─IdentifierSyntax
//@[004:011) | | └─Token(Identifier) |numbers|
//@[012:013) | ├─Token(Assignment) |=|
//@[014:025) | └─FunctionCallSyntax
//@[014:019) | | ├─IdentifierSyntax
//@[014:019) | | | └─Token(Identifier) |range|
//@[019:020) | | ├─Token(LeftParen) |(|
//@[020:022) | | ├─FunctionArgumentSyntax
//@[020:021) | | | ├─IntegerLiteralSyntax
//@[020:021) | | | | └─Token(Integer) |0|
//@[021:022) | | | └─Token(Comma) |,|
//@[023:024) | | ├─FunctionArgumentSyntax
//@[023:024) | | | └─IntegerLiteralSyntax
//@[023:024) | | | | └─Token(Integer) |4|
//@[024:025) | | └─Token(RightParen) |)|
//@[025:027) ├─Token(NewLine) |\n\n|

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[000:046) ├─VariableDeclarationSyntax
//@[000:003) | ├─Token(Identifier) |var|
//@[004:012) | ├─IdentifierSyntax
//@[004:012) | | └─Token(Identifier) |sayHello|
//@[013:014) | ├─Token(Assignment) |=|
//@[015:046) | └─FunctionCallSyntax
//@[015:018) | | ├─IdentifierSyntax
//@[015:018) | | | └─Token(Identifier) |map|
//@[018:019) | | ├─Token(LeftParen) |(|
//@[019:026) | | ├─FunctionArgumentSyntax
//@[019:025) | | | ├─VariableAccessSyntax
//@[019:025) | | | | └─IdentifierSyntax
//@[019:025) | | | | | └─Token(Identifier) |doggos|
//@[025:026) | | | └─Token(Comma) |,|
//@[027:045) | | ├─FunctionArgumentSyntax
//@[027:045) | | | └─LambdaSyntax
//@[027:028) | | | | ├─LocalVariableSyntax
//@[027:028) | | | | | └─IdentifierSyntax
//@[027:028) | | | | | | └─Token(Identifier) |i|
//@[029:031) | | | | ├─Token(Arrow) |=>|
//@[032:045) | | | | └─StringSyntax
//@[032:041) | | | | | ├─Token(StringLeftPiece) |'Hello ${|
//@[041:042) | | | | | ├─VariableAccessSyntax
//@[041:042) | | | | | | └─IdentifierSyntax
//@[041:042) | | | | | | | └─Token(Identifier) |i|
//@[042:045) | | | | | └─Token(StringRightPiece) |}!'|
//@[045:046) | | └─Token(RightParen) |)|
//@[046:048) ├─Token(NewLine) |\n\n|

var isEven = filter(numbers, i => 0 == i % 2)
//@[000:045) ├─VariableDeclarationSyntax
//@[000:003) | ├─Token(Identifier) |var|
//@[004:010) | ├─IdentifierSyntax
//@[004:010) | | └─Token(Identifier) |isEven|
//@[011:012) | ├─Token(Assignment) |=|
//@[013:045) | └─FunctionCallSyntax
//@[013:019) | | ├─IdentifierSyntax
//@[013:019) | | | └─Token(Identifier) |filter|
//@[019:020) | | ├─Token(LeftParen) |(|
//@[020:028) | | ├─FunctionArgumentSyntax
//@[020:027) | | | ├─VariableAccessSyntax
//@[020:027) | | | | └─IdentifierSyntax
//@[020:027) | | | | | └─Token(Identifier) |numbers|
//@[027:028) | | | └─Token(Comma) |,|
//@[029:044) | | ├─FunctionArgumentSyntax
//@[029:044) | | | └─LambdaSyntax
//@[029:030) | | | | ├─LocalVariableSyntax
//@[029:030) | | | | | └─IdentifierSyntax
//@[029:030) | | | | | | └─Token(Identifier) |i|
//@[031:033) | | | | ├─Token(Arrow) |=>|
//@[034:044) | | | | └─BinaryOperationSyntax
//@[034:035) | | | | | ├─IntegerLiteralSyntax
//@[034:035) | | | | | | └─Token(Integer) |0|
//@[036:038) | | | | | ├─Token(Equals) |==|
//@[039:044) | | | | | └─BinaryOperationSyntax
//@[039:040) | | | | | | ├─VariableAccessSyntax
//@[039:040) | | | | | | | └─IdentifierSyntax
//@[039:040) | | | | | | | | └─Token(Identifier) |i|
//@[041:042) | | | | | | ├─Token(Modulo) |%|
//@[043:044) | | | | | | └─IntegerLiteralSyntax
//@[043:044) | | | | | | | └─Token(Integer) |2|
//@[044:045) | | └─Token(RightParen) |)|
//@[045:047) ├─Token(NewLine) |\n\n|

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[000:118) ├─VariableDeclarationSyntax
//@[000:003) | ├─Token(Identifier) |var|
//@[004:027) | ├─IdentifierSyntax
//@[004:027) | | └─Token(Identifier) |evenDoggosNestedLambdas|
//@[028:029) | ├─Token(Assignment) |=|
//@[030:118) | └─FunctionCallSyntax
//@[030:033) | | ├─IdentifierSyntax
//@[030:033) | | | └─Token(Identifier) |map|
//@[033:034) | | ├─Token(LeftParen) |(|
//@[034:102) | | ├─FunctionArgumentSyntax
//@[034:101) | | | ├─FunctionCallSyntax
//@[034:040) | | | | ├─IdentifierSyntax
//@[034:040) | | | | | └─Token(Identifier) |filter|
//@[040:041) | | | | ├─Token(LeftParen) |(|
//@[041:049) | | | | ├─FunctionArgumentSyntax
//@[041:048) | | | | | ├─VariableAccessSyntax
//@[041:048) | | | | | | └─IdentifierSyntax
//@[041:048) | | | | | | | └─Token(Identifier) |numbers|
//@[048:049) | | | | | └─Token(Comma) |,|
//@[050:100) | | | | ├─FunctionArgumentSyntax
//@[050:100) | | | | | └─LambdaSyntax
//@[050:051) | | | | | | ├─LocalVariableSyntax
//@[050:051) | | | | | | | └─IdentifierSyntax
//@[050:051) | | | | | | | | └─Token(Identifier) |i|
//@[052:054) | | | | | | ├─Token(Arrow) |=>|
//@[055:100) | | | | | | └─FunctionCallSyntax
//@[055:063) | | | | | | | ├─IdentifierSyntax
//@[055:063) | | | | | | | | └─Token(Identifier) |contains|
//@[063:064) | | | | | | | ├─Token(LeftParen) |(|
//@[064:097) | | | | | | | ├─FunctionArgumentSyntax
//@[064:096) | | | | | | | | ├─FunctionCallSyntax
//@[064:070) | | | | | | | | | ├─IdentifierSyntax
//@[064:070) | | | | | | | | | | └─Token(Identifier) |filter|
//@[070:071) | | | | | | | | | ├─Token(LeftParen) |(|
//@[071:079) | | | | | | | | | ├─FunctionArgumentSyntax
//@[071:078) | | | | | | | | | | ├─VariableAccessSyntax
//@[071:078) | | | | | | | | | | | └─IdentifierSyntax
//@[071:078) | | | | | | | | | | | | └─Token(Identifier) |numbers|
//@[078:079) | | | | | | | | | | └─Token(Comma) |,|
//@[080:095) | | | | | | | | | ├─FunctionArgumentSyntax
//@[080:095) | | | | | | | | | | └─LambdaSyntax
//@[080:081) | | | | | | | | | | | ├─LocalVariableSyntax
//@[080:081) | | | | | | | | | | | | └─IdentifierSyntax
//@[080:081) | | | | | | | | | | | | | └─Token(Identifier) |j|
//@[082:084) | | | | | | | | | | | ├─Token(Arrow) |=>|
//@[085:095) | | | | | | | | | | | └─BinaryOperationSyntax
//@[085:086) | | | | | | | | | | | | ├─IntegerLiteralSyntax
//@[085:086) | | | | | | | | | | | | | └─Token(Integer) |0|
//@[087:089) | | | | | | | | | | | | ├─Token(Equals) |==|
//@[090:095) | | | | | | | | | | | | └─BinaryOperationSyntax
//@[090:091) | | | | | | | | | | | | | ├─VariableAccessSyntax
//@[090:091) | | | | | | | | | | | | | | └─IdentifierSyntax
//@[090:091) | | | | | | | | | | | | | | | └─Token(Identifier) |j|
//@[092:093) | | | | | | | | | | | | | ├─Token(Modulo) |%|
//@[094:095) | | | | | | | | | | | | | └─IntegerLiteralSyntax
//@[094:095) | | | | | | | | | | | | | | └─Token(Integer) |2|
//@[095:096) | | | | | | | | | └─Token(RightParen) |)|
//@[096:097) | | | | | | | | └─Token(Comma) |,|
//@[098:099) | | | | | | | ├─FunctionArgumentSyntax
//@[098:099) | | | | | | | | └─VariableAccessSyntax
//@[098:099) | | | | | | | | | └─IdentifierSyntax
//@[098:099) | | | | | | | | | | └─Token(Identifier) |i|
//@[099:100) | | | | | | | └─Token(RightParen) |)|
//@[100:101) | | | | └─Token(RightParen) |)|
//@[101:102) | | | └─Token(Comma) |,|
//@[103:117) | | ├─FunctionArgumentSyntax
//@[103:117) | | | └─LambdaSyntax
//@[103:104) | | | | ├─LocalVariableSyntax
//@[103:104) | | | | | └─IdentifierSyntax
//@[103:104) | | | | | | └─Token(Identifier) |x|
//@[105:107) | | | | ├─Token(Arrow) |=>|
//@[108:117) | | | | └─ArrayAccessSyntax
//@[108:114) | | | | | ├─VariableAccessSyntax
//@[108:114) | | | | | | └─IdentifierSyntax
//@[108:114) | | | | | | | └─Token(Identifier) |doggos|
//@[114:115) | | | | | ├─Token(LeftSquare) |[|
//@[115:116) | | | | | ├─VariableAccessSyntax
//@[115:116) | | | | | | └─IdentifierSyntax
//@[115:116) | | | | | | | └─Token(Identifier) |x|
//@[116:117) | | | | | └─Token(RightSquare) |]|
//@[117:118) | | └─Token(RightParen) |)|
//@[118:119) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
