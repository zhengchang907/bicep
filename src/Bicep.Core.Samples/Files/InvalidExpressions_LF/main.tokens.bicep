/*
  This tests the various cases of invalid expressions.
*/
//@[02:04) NewLine |\n\n|

// bad expressions
//@[18:19) NewLine |\n|
var bad = a+
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) Identifier |a|
//@[11:12) Plus |+|
//@[12:13) NewLine |\n|
var bad = *
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) Asterisk |*|
//@[11:12) NewLine |\n|
var bad = /
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) Slash |/|
//@[11:12) NewLine |\n|
var bad = %
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) Modulo |%|
//@[11:12) NewLine |\n|
var bad = 33-
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:12) Integer |33|
//@[12:13) Minus |-|
//@[13:14) NewLine |\n|
var bad = --33
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) Minus |-|
//@[11:12) Minus |-|
//@[12:14) Integer |33|
//@[14:15) NewLine |\n|
var bad = 3 * 4 /
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) Integer |3|
//@[12:13) Asterisk |*|
//@[14:15) Integer |4|
//@[16:17) Slash |/|
//@[17:18) NewLine |\n|
var bad = 222222222222222222222222222222222222222222 * 4
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:52) Integer |222222222222222222222222222222222222222222|
//@[53:54) Asterisk |*|
//@[55:56) Integer |4|
//@[56:57) NewLine |\n|
var bad = (null) ?
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[17:18) Question |?|
//@[18:19) NewLine |\n|
var bad = (null) ? :
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[17:18) Question |?|
//@[19:20) Colon |:|
//@[20:21) NewLine |\n|
var bad = (null) ? !
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[17:18) Question |?|
//@[19:20) Exclamation |!|
//@[20:21) NewLine |\n|
var bad = (null)!
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[16:17) Exclamation |!|
//@[17:18) NewLine |\n|
var bad = (null)[0]
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[16:17) LeftSquare |[|
//@[17:18) Integer |0|
//@[18:19) RightSquare |]|
//@[19:20) NewLine |\n|
var bad = ()
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
var bad = 
//@[00:03) Identifier |var|
//@[04:07) Identifier |bad|
//@[08:09) Assignment |=|
//@[10:12) NewLine |\n\n|

// variables not supported
//@[26:27) NewLine |\n|
var x = a + 2
//@[00:03) Identifier |var|
//@[04:05) Identifier |x|
//@[06:07) Assignment |=|
//@[08:09) Identifier |a|
//@[10:11) Plus |+|
//@[12:13) Integer |2|
//@[13:15) NewLine |\n\n|

// unary NOT
//@[12:13) NewLine |\n|
var not = !null
//@[00:03) Identifier |var|
//@[04:07) Identifier |not|
//@[08:09) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:15) NullKeyword |null|
//@[15:16) NewLine |\n|
var not = !4
//@[00:03) Identifier |var|
//@[04:07) Identifier |not|
//@[08:09) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:12) Integer |4|
//@[12:13) NewLine |\n|
var not = !'s'
//@[00:03) Identifier |var|
//@[04:07) Identifier |not|
//@[08:09) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:14) StringComplete |'s'|
//@[14:15) NewLine |\n|
var not = ![
//@[00:03) Identifier |var|
//@[04:07) Identifier |not|
//@[08:09) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
var not = !{
//@[00:03) Identifier |var|
//@[04:07) Identifier |not|
//@[08:09) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:12) LeftBrace |{|
//@[12:13) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// unary not chaining will be added in the future
//@[49:50) NewLine |\n|
var not = !!!!!!!true
//@[00:03) Identifier |var|
//@[04:07) Identifier |not|
//@[08:09) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:12) Exclamation |!|
//@[12:13) Exclamation |!|
//@[13:14) Exclamation |!|
//@[14:15) Exclamation |!|
//@[15:16) Exclamation |!|
//@[16:17) Exclamation |!|
//@[17:21) TrueKeyword |true|
//@[21:23) NewLine |\n\n|

// unary minus chaining will not be supported (to reserve -- in case we need it)
//@[80:81) NewLine |\n|
var minus = ------12
//@[00:03) Identifier |var|
//@[04:09) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:14) Minus |-|
//@[14:15) Minus |-|
//@[15:16) Minus |-|
//@[16:17) Minus |-|
//@[17:18) Minus |-|
//@[18:20) Integer |12|
//@[20:22) NewLine |\n\n|

// unary minus
//@[14:15) NewLine |\n|
var minus = -true
//@[00:03) Identifier |var|
//@[04:09) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:17) TrueKeyword |true|
//@[17:18) NewLine |\n|
var minus = -null
//@[00:03) Identifier |var|
//@[04:09) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:17) NullKeyword |null|
//@[17:18) NewLine |\n|
var minus = -'s'
//@[00:03) Identifier |var|
//@[04:09) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:16) StringComplete |'s'|
//@[16:17) NewLine |\n|
var minus = -[
//@[00:03) Identifier |var|
//@[04:09) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
var minus = -{
//@[00:03) Identifier |var|
//@[04:09) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:14) LeftBrace |{|
//@[14:15) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// multiplicative
//@[17:18) NewLine |\n|
var mod = 's' % true
//@[00:03) Identifier |var|
//@[04:07) Identifier |mod|
//@[08:09) Assignment |=|
//@[10:13) StringComplete |'s'|
//@[14:15) Modulo |%|
//@[16:20) TrueKeyword |true|
//@[20:21) NewLine |\n|
var mul = true * null
//@[00:03) Identifier |var|
//@[04:07) Identifier |mul|
//@[08:09) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[15:16) Asterisk |*|
//@[17:21) NullKeyword |null|
//@[21:22) NewLine |\n|
var div = {
//@[00:03) Identifier |var|
//@[04:07) Identifier |div|
//@[08:09) Assignment |=|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
} / [
//@[00:01) RightBrace |}|
//@[02:03) Slash |/|
//@[04:05) LeftSquare |[|
//@[05:06) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:03) NewLine |\n\n|

// additive
//@[11:12) NewLine |\n|
var add = null + 's'
//@[00:03) Identifier |var|
//@[04:07) Identifier |add|
//@[08:09) Assignment |=|
//@[10:14) NullKeyword |null|
//@[15:16) Plus |+|
//@[17:20) StringComplete |'s'|
//@[20:21) NewLine |\n|
var sub = true - false
//@[00:03) Identifier |var|
//@[04:07) Identifier |sub|
//@[08:09) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[15:16) Minus |-|
//@[17:22) FalseKeyword |false|
//@[22:23) NewLine |\n|
var add = 'bad' + 'str'
//@[00:03) Identifier |var|
//@[04:07) Identifier |add|
//@[08:09) Assignment |=|
//@[10:15) StringComplete |'bad'|
//@[16:17) Plus |+|
//@[18:23) StringComplete |'str'|
//@[23:25) NewLine |\n\n|

// equality (== and != can't have a type error because they work on "any" type)
//@[79:80) NewLine |\n|
var eq = true =~ null
//@[00:03) Identifier |var|
//@[04:06) Identifier |eq|
//@[07:08) Assignment |=|
//@[09:13) TrueKeyword |true|
//@[14:16) EqualsInsensitive |=~|
//@[17:21) NullKeyword |null|
//@[21:22) NewLine |\n|
var ne = 15 !~ [
//@[00:03) Identifier |var|
//@[04:06) Identifier |ne|
//@[07:08) Assignment |=|
//@[09:11) Integer |15|
//@[12:14) NotEqualsInsensitive |!~|
//@[15:16) LeftSquare |[|
//@[16:17) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:03) NewLine |\n\n|

// relational
//@[13:14) NewLine |\n|
var lt = 4 < 's'
//@[00:03) Identifier |var|
//@[04:06) Identifier |lt|
//@[07:08) Assignment |=|
//@[09:10) Integer |4|
//@[11:12) LessThan |<|
//@[13:16) StringComplete |'s'|
//@[16:17) NewLine |\n|
var lteq = null <= 10
//@[00:03) Identifier |var|
//@[04:08) Identifier |lteq|
//@[09:10) Assignment |=|
//@[11:15) NullKeyword |null|
//@[16:18) LessThanOrEqual |<=|
//@[19:21) Integer |10|
//@[21:22) NewLine |\n|
var gt = false>[
//@[00:03) Identifier |var|
//@[04:06) Identifier |gt|
//@[07:08) Assignment |=|
//@[09:14) FalseKeyword |false|
//@[14:15) GreaterThan |>|
//@[15:16) LeftSquare |[|
//@[16:17) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
var gteq = {
//@[00:03) Identifier |var|
//@[04:08) Identifier |gteq|
//@[09:10) Assignment |=|
//@[11:12) LeftBrace |{|
//@[12:13) NewLine |\n|
} >= false
//@[00:01) RightBrace |}|
//@[02:04) GreaterThanOrEqual |>=|
//@[05:10) FalseKeyword |false|
//@[10:12) NewLine |\n\n|

// logical
//@[10:11) NewLine |\n|
var and = null && 'a'
//@[00:03) Identifier |var|
//@[04:07) Identifier |and|
//@[08:09) Assignment |=|
//@[10:14) NullKeyword |null|
//@[15:17) LogicalAnd |&&|
//@[18:21) StringComplete |'a'|
//@[21:22) NewLine |\n|
var or = 10 || 4
//@[00:03) Identifier |var|
//@[04:06) Identifier |or|
//@[07:08) Assignment |=|
//@[09:11) Integer |10|
//@[12:14) LogicalOr ||||
//@[15:16) Integer |4|
//@[16:18) NewLine |\n\n|

// conditional
//@[14:15) NewLine |\n|
var ternary = null ? 4 : false
//@[00:03) Identifier |var|
//@[04:11) Identifier |ternary|
//@[12:13) Assignment |=|
//@[14:18) NullKeyword |null|
//@[19:20) Question |?|
//@[21:22) Integer |4|
//@[23:24) Colon |:|
//@[25:30) FalseKeyword |false|
//@[30:32) NewLine |\n\n|

// complex expressions
//@[22:23) NewLine |\n|
var complex = test(2 + 3*4, true || false && null)
//@[00:03) Identifier |var|
//@[04:11) Identifier |complex|
//@[12:13) Assignment |=|
//@[14:18) Identifier |test|
//@[18:19) LeftParen |(|
//@[19:20) Integer |2|
//@[21:22) Plus |+|
//@[23:24) Integer |3|
//@[24:25) Asterisk |*|
//@[25:26) Integer |4|
//@[26:27) Comma |,|
//@[28:32) TrueKeyword |true|
//@[33:35) LogicalOr ||||
//@[36:41) FalseKeyword |false|
//@[42:44) LogicalAnd |&&|
//@[45:49) NullKeyword |null|
//@[49:50) RightParen |)|
//@[50:51) NewLine |\n|
var complex = -2 && 3 && !4 && 5
//@[00:03) Identifier |var|
//@[04:11) Identifier |complex|
//@[12:13) Assignment |=|
//@[14:15) Minus |-|
//@[15:16) Integer |2|
//@[17:19) LogicalAnd |&&|
//@[20:21) Integer |3|
//@[22:24) LogicalAnd |&&|
//@[25:26) Exclamation |!|
//@[26:27) Integer |4|
//@[28:30) LogicalAnd |&&|
//@[31:32) Integer |5|
//@[32:33) NewLine |\n|
var complex = null ? !4: false
//@[00:03) Identifier |var|
//@[04:11) Identifier |complex|
//@[12:13) Assignment |=|
//@[14:18) NullKeyword |null|
//@[19:20) Question |?|
//@[21:22) Exclamation |!|
//@[22:23) Integer |4|
//@[23:24) Colon |:|
//@[25:30) FalseKeyword |false|
//@[30:31) NewLine |\n|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[00:03) Identifier |var|
//@[04:11) Identifier |complex|
//@[12:13) Assignment |=|
//@[14:18) TrueKeyword |true|
//@[19:21) Equals |==|
//@[22:27) FalseKeyword |false|
//@[28:30) NotEquals |!=|
//@[31:35) NullKeyword |null|
//@[36:38) Equals |==|
//@[39:40) Integer |4|
//@[41:43) NotEquals |!=|
//@[44:47) StringComplete |'a'|
//@[48:49) Question |?|
//@[50:51) Minus |-|
//@[51:52) Integer |2|
//@[53:55) LogicalAnd |&&|
//@[56:57) Integer |3|
//@[58:60) LogicalAnd |&&|
//@[61:62) Exclamation |!|
//@[62:63) Integer |4|
//@[64:66) LogicalAnd |&&|
//@[67:68) Integer |5|
//@[69:70) Colon |:|
//@[71:75) TrueKeyword |true|
//@[76:78) LogicalOr ||||
//@[79:84) FalseKeyword |false|
//@[85:87) LogicalAnd |&&|
//@[88:92) NullKeyword |null|
//@[92:94) NewLine |\n\n|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[00:03) Identifier |var|
//@[04:17) Identifier |nestedTernary|
//@[18:19) Assignment |=|
//@[20:24) NullKeyword |null|
//@[25:26) Question |?|
//@[27:28) Integer |1|
//@[29:30) Colon |:|
//@[31:32) Integer |2|
//@[33:34) Question |?|
//@[35:39) TrueKeyword |true|
//@[40:41) Question |?|
//@[42:45) StringComplete |'a'|
//@[45:46) Colon |:|
//@[47:50) StringComplete |'b'|
//@[51:52) Colon |:|
//@[53:58) FalseKeyword |false|
//@[59:60) Question |?|
//@[61:64) StringComplete |'d'|
//@[65:66) Colon |:|
//@[67:69) Integer |15|
//@[69:70) NewLine |\n|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[00:03) Identifier |var|
//@[04:17) Identifier |nestedTernary|
//@[18:19) Assignment |=|
//@[20:21) LeftParen |(|
//@[21:25) NullKeyword |null|
//@[26:27) Question |?|
//@[28:29) Integer |1|
//@[30:31) Colon |:|
//@[32:33) Integer |2|
//@[33:34) RightParen |)|
//@[35:36) Question |?|
//@[37:38) LeftParen |(|
//@[38:42) TrueKeyword |true|
//@[43:44) Question |?|
//@[45:48) StringComplete |'a'|
//@[48:49) Colon |:|
//@[50:53) StringComplete |'b'|
//@[53:54) RightParen |)|
//@[55:56) Colon |:|
//@[57:58) LeftParen |(|
//@[58:63) FalseKeyword |false|
//@[64:65) Question |?|
//@[66:69) StringComplete |'d'|
//@[70:71) Colon |:|
//@[72:74) Integer |15|
//@[74:75) RightParen |)|
//@[75:77) NewLine |\n\n|

// bad array access
//@[19:20) NewLine |\n|
var errorInsideArrayAccess = [
//@[00:03) Identifier |var|
//@[04:26) Identifier |errorInsideArrayAccess|
//@[27:28) Assignment |=|
//@[29:30) LeftSquare |[|
//@[30:31) NewLine |\n|
  !null
//@[02:03) Exclamation |!|
//@[03:07) NullKeyword |null|
//@[07:08) NewLine |\n|
][!0]
//@[00:01) RightSquare |]|
//@[01:02) LeftSquare |[|
//@[02:03) Exclamation |!|
//@[03:04) Integer |0|
//@[04:05) RightSquare |]|
//@[05:06) NewLine |\n|
var integerIndexOnNonArray = (null)[0]
//@[00:03) Identifier |var|
//@[04:26) Identifier |integerIndexOnNonArray|
//@[27:28) Assignment |=|
//@[29:30) LeftParen |(|
//@[30:34) NullKeyword |null|
//@[34:35) RightParen |)|
//@[35:36) LeftSquare |[|
//@[36:37) Integer |0|
//@[37:38) RightSquare |]|
//@[38:39) NewLine |\n|
var stringIndexOnNonObject = 'test'['test']
//@[00:03) Identifier |var|
//@[04:26) Identifier |stringIndexOnNonObject|
//@[27:28) Assignment |=|
//@[29:35) StringComplete |'test'|
//@[35:36) LeftSquare |[|
//@[36:42) StringComplete |'test'|
//@[42:43) RightSquare |]|
//@[43:44) NewLine |\n|
var malformedStringIndex = {
//@[00:03) Identifier |var|
//@[04:24) Identifier |malformedStringIndex|
//@[25:26) Assignment |=|
//@[27:28) LeftBrace |{|
//@[28:29) NewLine |\n|
}['test\e']
//@[00:01) RightBrace |}|
//@[01:02) LeftSquare |[|
//@[02:10) StringComplete |'test\e'|
//@[10:11) RightSquare |]|
//@[11:12) NewLine |\n|
var invalidIndexTypeOverAny = any(true)[true]
//@[00:03) Identifier |var|
//@[04:27) Identifier |invalidIndexTypeOverAny|
//@[28:29) Assignment |=|
//@[30:33) Identifier |any|
//@[33:34) LeftParen |(|
//@[34:38) TrueKeyword |true|
//@[38:39) RightParen |)|
//@[39:40) LeftSquare |[|
//@[40:44) TrueKeyword |true|
//@[44:45) RightSquare |]|
//@[45:46) NewLine |\n|
var badIndexOverArray = [][null]
//@[00:03) Identifier |var|
//@[04:21) Identifier |badIndexOverArray|
//@[22:23) Assignment |=|
//@[24:25) LeftSquare |[|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:31) NullKeyword |null|
//@[31:32) RightSquare |]|
//@[32:33) NewLine |\n|
var badIndexOverArray2 = []['s']
//@[00:03) Identifier |var|
//@[04:22) Identifier |badIndexOverArray2|
//@[23:24) Assignment |=|
//@[25:26) LeftSquare |[|
//@[26:27) RightSquare |]|
//@[27:28) LeftSquare |[|
//@[28:31) StringComplete |'s'|
//@[31:32) RightSquare |]|
//@[32:33) NewLine |\n|
var badIndexOverObj = {}[true]
//@[00:03) Identifier |var|
//@[04:19) Identifier |badIndexOverObj|
//@[20:21) Assignment |=|
//@[22:23) LeftBrace |{|
//@[23:24) RightBrace |}|
//@[24:25) LeftSquare |[|
//@[25:29) TrueKeyword |true|
//@[29:30) RightSquare |]|
//@[30:31) NewLine |\n|
var badIndexOverObj2 = {}[0]
//@[00:03) Identifier |var|
//@[04:20) Identifier |badIndexOverObj2|
//@[21:22) Assignment |=|
//@[23:24) LeftBrace |{|
//@[24:25) RightBrace |}|
//@[25:26) LeftSquare |[|
//@[26:27) Integer |0|
//@[27:28) RightSquare |]|
//@[28:29) NewLine |\n|
var badExpressionIndexer = {}[base64('a')]
//@[00:03) Identifier |var|
//@[04:24) Identifier |badExpressionIndexer|
//@[25:26) Assignment |=|
//@[27:28) LeftBrace |{|
//@[28:29) RightBrace |}|
//@[29:30) LeftSquare |[|
//@[30:36) Identifier |base64|
//@[36:37) LeftParen |(|
//@[37:40) StringComplete |'a'|
//@[40:41) RightParen |)|
//@[41:42) RightSquare |]|
//@[42:44) NewLine |\n\n|

// bad propertyAccess
//@[21:22) NewLine |\n|
var dotAccessOnNonObject = true.foo
//@[00:03) Identifier |var|
//@[04:24) Identifier |dotAccessOnNonObject|
//@[25:26) Assignment |=|
//@[27:31) TrueKeyword |true|
//@[31:32) Dot |.|
//@[32:35) Identifier |foo|
//@[35:36) NewLine |\n|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[00:03) Identifier |var|
//@[04:33) Identifier |badExpressionInPropertyAccess|
//@[34:35) Assignment |=|
//@[36:49) Identifier |resourceGroup|
//@[49:50) LeftParen |(|
//@[50:51) RightParen |)|
//@[51:52) LeftSquare |[|
//@[52:53) Exclamation |!|
//@[53:63) StringComplete |'location'|
//@[63:64) RightSquare |]|
//@[64:66) NewLine |\n\n|

var propertyAccessOnVariable = x.foo
//@[00:03) Identifier |var|
//@[04:28) Identifier |propertyAccessOnVariable|
//@[29:30) Assignment |=|
//@[31:32) Identifier |x|
//@[32:33) Dot |.|
//@[33:36) Identifier |foo|
//@[36:38) NewLine |\n\n|

// missing property in property access
//@[38:39) NewLine |\n|
var oneValidDeclaration = {}
//@[00:03) Identifier |var|
//@[04:23) Identifier |oneValidDeclaration|
//@[24:25) Assignment |=|
//@[26:27) LeftBrace |{|
//@[27:28) RightBrace |}|
//@[28:29) NewLine |\n|
var missingPropertyName = oneValidDeclaration.
//@[00:03) Identifier |var|
//@[04:23) Identifier |missingPropertyName|
//@[24:25) Assignment |=|
//@[26:45) Identifier |oneValidDeclaration|
//@[45:46) Dot |.|
//@[46:47) NewLine |\n|
var missingPropertyInsideAnExpression = oneValidDeclaration. + oneValidDeclaration.
//@[00:03) Identifier |var|
//@[04:37) Identifier |missingPropertyInsideAnExpression|
//@[38:39) Assignment |=|
//@[40:59) Identifier |oneValidDeclaration|
//@[59:60) Dot |.|
//@[61:62) Plus |+|
//@[63:82) Identifier |oneValidDeclaration|
//@[82:83) Dot |.|
//@[83:85) NewLine |\n\n|

// function used like a variable
//@[32:33) NewLine |\n|
var funcvarvar = concat + base64 || !uniqueString
//@[00:03) Identifier |var|
//@[04:14) Identifier |funcvarvar|
//@[15:16) Assignment |=|
//@[17:23) Identifier |concat|
//@[24:25) Plus |+|
//@[26:32) Identifier |base64|
//@[33:35) LogicalOr ||||
//@[36:37) Exclamation |!|
//@[37:49) Identifier |uniqueString|
//@[49:50) NewLine |\n|
param funcvarparam bool = concat
//@[00:05) Identifier |param|
//@[06:18) Identifier |funcvarparam|
//@[19:23) Identifier |bool|
//@[24:25) Assignment |=|
//@[26:32) Identifier |concat|
//@[32:33) NewLine |\n|
output funcvarout array = padLeft
//@[00:06) Identifier |output|
//@[07:17) Identifier |funcvarout|
//@[18:23) Identifier |array|
//@[24:25) Assignment |=|
//@[26:33) Identifier |padLeft|
//@[33:35) NewLine |\n\n|

// non-existent function
//@[24:25) NewLine |\n|
var fakeFunc = red() + green() * orange()
//@[00:03) Identifier |var|
//@[04:12) Identifier |fakeFunc|
//@[13:14) Assignment |=|
//@[15:18) Identifier |red|
//@[18:19) LeftParen |(|
//@[19:20) RightParen |)|
//@[21:22) Plus |+|
//@[23:28) Identifier |green|
//@[28:29) LeftParen |(|
//@[29:30) RightParen |)|
//@[31:32) Asterisk |*|
//@[33:39) Identifier |orange|
//@[39:40) LeftParen |(|
//@[40:41) RightParen |)|
//@[41:42) NewLine |\n|
param fakeFuncP string = blue()
//@[00:05) Identifier |param|
//@[06:15) Identifier |fakeFuncP|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:29) Identifier |blue|
//@[29:30) LeftParen |(|
//@[30:31) RightParen |)|
//@[31:33) NewLine |\n\n|

// non-existent variable
//@[24:25) NewLine |\n|
var fakeVar = concat(totallyFakeVar, 's')
//@[00:03) Identifier |var|
//@[04:11) Identifier |fakeVar|
//@[12:13) Assignment |=|
//@[14:20) Identifier |concat|
//@[20:21) LeftParen |(|
//@[21:35) Identifier |totallyFakeVar|
//@[35:36) Comma |,|
//@[37:40) StringComplete |'s'|
//@[40:41) RightParen |)|
//@[41:43) NewLine |\n\n|

// bad functions arguments
//@[26:27) NewLine |\n|
var concatNotEnough = concat()
//@[00:03) Identifier |var|
//@[04:19) Identifier |concatNotEnough|
//@[20:21) Assignment |=|
//@[22:28) Identifier |concat|
//@[28:29) LeftParen |(|
//@[29:30) RightParen |)|
//@[30:31) NewLine |\n|
var padLeftNotEnough = padLeft('s')
//@[00:03) Identifier |var|
//@[04:20) Identifier |padLeftNotEnough|
//@[21:22) Assignment |=|
//@[23:30) Identifier |padLeft|
//@[30:31) LeftParen |(|
//@[31:34) StringComplete |'s'|
//@[34:35) RightParen |)|
//@[35:36) NewLine |\n|
var takeTooMany = take([
//@[00:03) Identifier |var|
//@[04:15) Identifier |takeTooMany|
//@[16:17) Assignment |=|
//@[18:22) Identifier |take|
//@[22:23) LeftParen |(|
//@[23:24) LeftSquare |[|
//@[24:25) NewLine |\n|
],1,2,'s')
//@[00:01) RightSquare |]|
//@[01:02) Comma |,|
//@[02:03) Integer |1|
//@[03:04) Comma |,|
//@[04:05) Integer |2|
//@[05:06) Comma |,|
//@[06:09) StringComplete |'s'|
//@[09:10) RightParen |)|
//@[10:12) NewLine |\n\n|

// missing arguments
//@[20:21) NewLine |\n|
var trailingArgumentComma = format('s',)
//@[00:03) Identifier |var|
//@[04:25) Identifier |trailingArgumentComma|
//@[26:27) Assignment |=|
//@[28:34) Identifier |format|
//@[34:35) LeftParen |(|
//@[35:38) StringComplete |'s'|
//@[38:39) Comma |,|
//@[39:40) RightParen |)|
//@[40:41) NewLine |\n|
var onlyArgumentComma = concat(,)
//@[00:03) Identifier |var|
//@[04:21) Identifier |onlyArgumentComma|
//@[22:23) Assignment |=|
//@[24:30) Identifier |concat|
//@[30:31) LeftParen |(|
//@[31:32) Comma |,|
//@[32:33) RightParen |)|
//@[33:34) NewLine |\n|
var multipleArgumentCommas = concat(,,,,,)
//@[00:03) Identifier |var|
//@[04:26) Identifier |multipleArgumentCommas|
//@[27:28) Assignment |=|
//@[29:35) Identifier |concat|
//@[35:36) LeftParen |(|
//@[36:37) Comma |,|
//@[37:38) Comma |,|
//@[38:39) Comma |,|
//@[39:40) Comma |,|
//@[40:41) Comma |,|
//@[41:42) RightParen |)|
//@[42:43) NewLine |\n|
var emptyArgInBetween = concat(true,,false)
//@[00:03) Identifier |var|
//@[04:21) Identifier |emptyArgInBetween|
//@[22:23) Assignment |=|
//@[24:30) Identifier |concat|
//@[30:31) LeftParen |(|
//@[31:35) TrueKeyword |true|
//@[35:36) Comma |,|
//@[36:37) Comma |,|
//@[37:42) FalseKeyword |false|
//@[42:43) RightParen |)|
//@[43:44) NewLine |\n|
var leadingEmptyArg = concat(,[])
//@[00:03) Identifier |var|
//@[04:19) Identifier |leadingEmptyArg|
//@[20:21) Assignment |=|
//@[22:28) Identifier |concat|
//@[28:29) LeftParen |(|
//@[29:30) Comma |,|
//@[30:31) LeftSquare |[|
//@[31:32) RightSquare |]|
//@[32:33) RightParen |)|
//@[33:34) NewLine |\n|
var leadingAndTrailingEmptyArg = concat(,'s',)
//@[00:03) Identifier |var|
//@[04:30) Identifier |leadingAndTrailingEmptyArg|
//@[31:32) Assignment |=|
//@[33:39) Identifier |concat|
//@[39:40) LeftParen |(|
//@[40:41) Comma |,|
//@[41:44) StringComplete |'s'|
//@[44:45) Comma |,|
//@[45:46) RightParen |)|
//@[46:48) NewLine |\n\n|

// wrong argument types
//@[23:24) NewLine |\n|
var concatWrongTypes = concat({
//@[00:03) Identifier |var|
//@[04:20) Identifier |concatWrongTypes|
//@[21:22) Assignment |=|
//@[23:29) Identifier |concat|
//@[29:30) LeftParen |(|
//@[30:31) LeftBrace |{|
//@[31:32) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
var concatWrongTypesContradiction = concat('s', [
//@[00:03) Identifier |var|
//@[04:33) Identifier |concatWrongTypesContradiction|
//@[34:35) Assignment |=|
//@[36:42) Identifier |concat|
//@[42:43) LeftParen |(|
//@[43:46) StringComplete |'s'|
//@[46:47) Comma |,|
//@[48:49) LeftSquare |[|
//@[49:50) NewLine |\n|
])
//@[00:01) RightSquare |]|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
var indexOfWrongTypes = indexOf(1,1)
//@[00:03) Identifier |var|
//@[04:21) Identifier |indexOfWrongTypes|
//@[22:23) Assignment |=|
//@[24:31) Identifier |indexOf|
//@[31:32) LeftParen |(|
//@[32:33) Integer |1|
//@[33:34) Comma |,|
//@[34:35) Integer |1|
//@[35:36) RightParen |)|
//@[36:38) NewLine |\n\n|

// not enough params
//@[20:21) NewLine |\n|
var test1 = listKeys('abcd')
//@[00:03) Identifier |var|
//@[04:09) Identifier |test1|
//@[10:11) Assignment |=|
//@[12:20) Identifier |listKeys|
//@[20:21) LeftParen |(|
//@[21:27) StringComplete |'abcd'|
//@[27:28) RightParen |)|
//@[28:30) NewLine |\n\n|

// list spelled wrong 
//@[22:23) NewLine |\n|
var test2 = lsitKeys('abcd', '2020-01-01')
//@[00:03) Identifier |var|
//@[04:09) Identifier |test2|
//@[10:11) Assignment |=|
//@[12:20) Identifier |lsitKeys|
//@[20:21) LeftParen |(|
//@[21:27) StringComplete |'abcd'|
//@[27:28) Comma |,|
//@[29:41) StringComplete |'2020-01-01'|
//@[41:42) RightParen |)|
//@[42:44) NewLine |\n\n|

// just 'lis' instead of 'list'
//@[31:32) NewLine |\n|
var test3 = lis('abcd', '2020-01-01')
//@[00:03) Identifier |var|
//@[04:09) Identifier |test3|
//@[10:11) Assignment |=|
//@[12:15) Identifier |lis|
//@[15:16) LeftParen |(|
//@[16:22) StringComplete |'abcd'|
//@[22:23) Comma |,|
//@[24:36) StringComplete |'2020-01-01'|
//@[36:37) RightParen |)|
//@[37:39) NewLine |\n\n|

var sampleObject = {
//@[00:03) Identifier |var|
//@[04:16) Identifier |sampleObject|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:21) NewLine |\n|
  myInt: 42
//@[02:07) Identifier |myInt|
//@[07:08) Colon |:|
//@[09:11) Integer |42|
//@[11:12) NewLine |\n|
  myStr: 's'
//@[02:07) Identifier |myStr|
//@[07:08) Colon |:|
//@[09:12) StringComplete |'s'|
//@[12:13) NewLine |\n|
  myBool: false
//@[02:08) Identifier |myBool|
//@[08:09) Colon |:|
//@[10:15) FalseKeyword |false|
//@[15:16) NewLine |\n|
  myNull: null
//@[02:08) Identifier |myNull|
//@[08:09) Colon |:|
//@[10:14) NullKeyword |null|
//@[14:15) NewLine |\n|
  myInner: {
//@[02:09) Identifier |myInner|
//@[09:10) Colon |:|
//@[11:12) LeftBrace |{|
//@[12:13) NewLine |\n|
    anotherStr: 'a'
//@[04:14) Identifier |anotherStr|
//@[14:15) Colon |:|
//@[16:19) StringComplete |'a'|
//@[19:20) NewLine |\n|
    otherArr: [
//@[04:12) Identifier |otherArr|
//@[12:13) Colon |:|
//@[14:15) LeftSquare |[|
//@[15:16) NewLine |\n|
      's'
//@[06:09) StringComplete |'s'|
//@[09:10) NewLine |\n|
      'a'
//@[06:09) StringComplete |'a'|
//@[09:10) NewLine |\n|
    ]
//@[04:05) RightSquare |]|
//@[05:06) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  myArr: [
//@[02:07) Identifier |myArr|
//@[07:08) Colon |:|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    1
//@[04:05) Integer |1|
//@[05:06) NewLine |\n|
    2
//@[04:05) Integer |2|
//@[05:06) NewLine |\n|
    3
//@[04:05) Integer |3|
//@[05:06) NewLine |\n|
  ]
//@[02:03) RightSquare |]|
//@[03:04) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

var badProperty = sampleObject.myFake
//@[00:03) Identifier |var|
//@[04:15) Identifier |badProperty|
//@[16:17) Assignment |=|
//@[18:30) Identifier |sampleObject|
//@[30:31) Dot |.|
//@[31:37) Identifier |myFake|
//@[37:38) NewLine |\n|
var badSpelling = sampleObject.myNul
//@[00:03) Identifier |var|
//@[04:15) Identifier |badSpelling|
//@[16:17) Assignment |=|
//@[18:30) Identifier |sampleObject|
//@[30:31) Dot |.|
//@[31:36) Identifier |myNul|
//@[36:37) NewLine |\n|
var badPropertyIndexer = sampleObject['fake']
//@[00:03) Identifier |var|
//@[04:22) Identifier |badPropertyIndexer|
//@[23:24) Assignment |=|
//@[25:37) Identifier |sampleObject|
//@[37:38) LeftSquare |[|
//@[38:44) StringComplete |'fake'|
//@[44:45) RightSquare |]|
//@[45:46) NewLine |\n|
var badType = sampleObject.myStr / 32
//@[00:03) Identifier |var|
//@[04:11) Identifier |badType|
//@[12:13) Assignment |=|
//@[14:26) Identifier |sampleObject|
//@[26:27) Dot |.|
//@[27:32) Identifier |myStr|
//@[33:34) Slash |/|
//@[35:37) Integer |32|
//@[37:38) NewLine |\n|
var badInnerProperty = sampleObject.myInner.fake
//@[00:03) Identifier |var|
//@[04:20) Identifier |badInnerProperty|
//@[21:22) Assignment |=|
//@[23:35) Identifier |sampleObject|
//@[35:36) Dot |.|
//@[36:43) Identifier |myInner|
//@[43:44) Dot |.|
//@[44:48) Identifier |fake|
//@[48:49) NewLine |\n|
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[00:03) Identifier |var|
//@[04:16) Identifier |badInnerType|
//@[17:18) Assignment |=|
//@[19:31) Identifier |sampleObject|
//@[31:32) Dot |.|
//@[32:39) Identifier |myInner|
//@[39:40) Dot |.|
//@[40:50) Identifier |anotherStr|
//@[51:52) Plus |+|
//@[53:54) Integer |2|
//@[54:55) NewLine |\n|
var badArrayIndexer = sampleObject.myArr['s']
//@[00:03) Identifier |var|
//@[04:19) Identifier |badArrayIndexer|
//@[20:21) Assignment |=|
//@[22:34) Identifier |sampleObject|
//@[34:35) Dot |.|
//@[35:40) Identifier |myArr|
//@[40:41) LeftSquare |[|
//@[41:44) StringComplete |'s'|
//@[44:45) RightSquare |]|
//@[45:46) NewLine |\n|
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[00:03) Identifier |var|
//@[04:24) Identifier |badInnerArrayIndexer|
//@[25:26) Assignment |=|
//@[27:39) Identifier |sampleObject|
//@[39:40) Dot |.|
//@[40:47) Identifier |myInner|
//@[47:48) Dot |.|
//@[48:56) Identifier |otherArr|
//@[56:57) LeftSquare |[|
//@[57:60) StringComplete |'s'|
//@[60:61) RightSquare |]|
//@[61:62) NewLine |\n|
var badIndexer = sampleObject.myStr['s']
//@[00:03) Identifier |var|
//@[04:14) Identifier |badIndexer|
//@[15:16) Assignment |=|
//@[17:29) Identifier |sampleObject|
//@[29:30) Dot |.|
//@[30:35) Identifier |myStr|
//@[35:36) LeftSquare |[|
//@[36:39) StringComplete |'s'|
//@[39:40) RightSquare |]|
//@[40:41) NewLine |\n|
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[00:03) Identifier |var|
//@[04:17) Identifier |badInnerArray|
//@[18:19) Assignment |=|
//@[20:32) Identifier |sampleObject|
//@[32:33) Dot |.|
//@[33:40) Identifier |myInner|
//@[40:41) Dot |.|
//@[41:48) Identifier |fakeArr|
//@[48:49) LeftSquare |[|
//@[49:52) StringComplete |'s'|
//@[52:53) RightSquare |]|
//@[53:54) NewLine |\n|
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
//@[00:03) Identifier |var|
//@[04:47) Identifier |invalidPropertyCallOnInstanceFunctionAccess|
//@[48:49) Assignment |=|
//@[50:51) Identifier |a|
//@[51:52) Dot |.|
//@[52:53) Identifier |b|
//@[53:54) Dot |.|
//@[54:55) Identifier |c|
//@[55:56) Dot |.|
//@[56:59) Identifier |bar|
//@[59:60) LeftParen |(|
//@[60:61) RightParen |)|
//@[61:62) Dot |.|
//@[62:65) Identifier |baz|
//@[65:66) NewLine |\n|
var invalidInstanceFunctionAccess = a.b.c.bar()
//@[00:03) Identifier |var|
//@[04:33) Identifier |invalidInstanceFunctionAccess|
//@[34:35) Assignment |=|
//@[36:37) Identifier |a|
//@[37:38) Dot |.|
//@[38:39) Identifier |b|
//@[39:40) Dot |.|
//@[40:41) Identifier |c|
//@[41:42) Dot |.|
//@[42:45) Identifier |bar|
//@[45:46) LeftParen |(|
//@[46:47) RightParen |)|
//@[47:48) NewLine |\n|
var invalidInstanceFunctionCall = az.az()
//@[00:03) Identifier |var|
//@[04:31) Identifier |invalidInstanceFunctionCall|
//@[32:33) Assignment |=|
//@[34:36) Identifier |az|
//@[36:37) Dot |.|
//@[37:39) Identifier |az|
//@[39:40) LeftParen |(|
//@[40:41) RightParen |)|
//@[41:42) NewLine |\n|
var invalidPropertyAccessOnAzNamespace = az.az
//@[00:03) Identifier |var|
//@[04:38) Identifier |invalidPropertyAccessOnAzNamespace|
//@[39:40) Assignment |=|
//@[41:43) Identifier |az|
//@[43:44) Dot |.|
//@[44:46) Identifier |az|
//@[46:47) NewLine |\n|
var invalidPropertyAccessOnSysNamespace = sys.az
//@[00:03) Identifier |var|
//@[04:39) Identifier |invalidPropertyAccessOnSysNamespace|
//@[40:41) Assignment |=|
//@[42:45) Identifier |sys|
//@[45:46) Dot |.|
//@[46:48) Identifier |az|
//@[48:49) NewLine |\n|
var invalidOperands = 1 + az
//@[00:03) Identifier |var|
//@[04:19) Identifier |invalidOperands|
//@[20:21) Assignment |=|
//@[22:23) Integer |1|
//@[24:25) Plus |+|
//@[26:28) Identifier |az|
//@[28:29) NewLine |\n|
var invalidStringAddition = 'hello' + sampleObject.myStr
//@[00:03) Identifier |var|
//@[04:25) Identifier |invalidStringAddition|
//@[26:27) Assignment |=|
//@[28:35) StringComplete |'hello'|
//@[36:37) Plus |+|
//@[38:50) Identifier |sampleObject|
//@[50:51) Dot |.|
//@[51:56) Identifier |myStr|
//@[56:58) NewLine |\n\n|

var bannedFunctions = {
//@[00:03) Identifier |var|
//@[04:19) Identifier |bannedFunctions|
//@[20:21) Assignment |=|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
  var: variables()
//@[02:05) Identifier |var|
//@[05:06) Colon |:|
//@[07:16) Identifier |variables|
//@[16:17) LeftParen |(|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
  param: parameters() + 2
//@[02:07) Identifier |param|
//@[07:08) Colon |:|
//@[09:19) Identifier |parameters|
//@[19:20) LeftParen |(|
//@[20:21) RightParen |)|
//@[22:23) Plus |+|
//@[24:25) Integer |2|
//@[25:26) NewLine |\n|
  if: sys.if(null,null)
//@[02:04) Identifier |if|
//@[04:05) Colon |:|
//@[06:09) Identifier |sys|
//@[09:10) Dot |.|
//@[10:12) Identifier |if|
//@[12:13) LeftParen |(|
//@[13:17) NullKeyword |null|
//@[17:18) Comma |,|
//@[18:22) NullKeyword |null|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
  obj: sys.createArray()
//@[02:05) Identifier |obj|
//@[05:06) Colon |:|
//@[07:10) Identifier |sys|
//@[10:11) Dot |.|
//@[11:22) Identifier |createArray|
//@[22:23) LeftParen |(|
//@[23:24) RightParen |)|
//@[24:25) NewLine |\n|
  arr: sys.createObject()
//@[02:05) Identifier |arr|
//@[05:06) Colon |:|
//@[07:10) Identifier |sys|
//@[10:11) Dot |.|
//@[11:23) Identifier |createObject|
//@[23:24) LeftParen |(|
//@[24:25) RightParen |)|
//@[25:26) NewLine |\n|
  numeric: sys.add(1) + sys.sub(2,3) + sys.mul(8,'s') + sys.div(true) + sys.mod(null, false)
//@[02:09) Identifier |numeric|
//@[09:10) Colon |:|
//@[11:14) Identifier |sys|
//@[14:15) Dot |.|
//@[15:18) Identifier |add|
//@[18:19) LeftParen |(|
//@[19:20) Integer |1|
//@[20:21) RightParen |)|
//@[22:23) Plus |+|
//@[24:27) Identifier |sys|
//@[27:28) Dot |.|
//@[28:31) Identifier |sub|
//@[31:32) LeftParen |(|
//@[32:33) Integer |2|
//@[33:34) Comma |,|
//@[34:35) Integer |3|
//@[35:36) RightParen |)|
//@[37:38) Plus |+|
//@[39:42) Identifier |sys|
//@[42:43) Dot |.|
//@[43:46) Identifier |mul|
//@[46:47) LeftParen |(|
//@[47:48) Integer |8|
//@[48:49) Comma |,|
//@[49:52) StringComplete |'s'|
//@[52:53) RightParen |)|
//@[54:55) Plus |+|
//@[56:59) Identifier |sys|
//@[59:60) Dot |.|
//@[60:63) Identifier |div|
//@[63:64) LeftParen |(|
//@[64:68) TrueKeyword |true|
//@[68:69) RightParen |)|
//@[70:71) Plus |+|
//@[72:75) Identifier |sys|
//@[75:76) Dot |.|
//@[76:79) Identifier |mod|
//@[79:80) LeftParen |(|
//@[80:84) NullKeyword |null|
//@[84:85) Comma |,|
//@[86:91) FalseKeyword |false|
//@[91:92) RightParen |)|
//@[92:93) NewLine |\n|
  relational: sys.less() && sys.lessOrEquals() && sys.greater() && sys.greaterOrEquals()
//@[02:12) Identifier |relational|
//@[12:13) Colon |:|
//@[14:17) Identifier |sys|
//@[17:18) Dot |.|
//@[18:22) Identifier |less|
//@[22:23) LeftParen |(|
//@[23:24) RightParen |)|
//@[25:27) LogicalAnd |&&|
//@[28:31) Identifier |sys|
//@[31:32) Dot |.|
//@[32:44) Identifier |lessOrEquals|
//@[44:45) LeftParen |(|
//@[45:46) RightParen |)|
//@[47:49) LogicalAnd |&&|
//@[50:53) Identifier |sys|
//@[53:54) Dot |.|
//@[54:61) Identifier |greater|
//@[61:62) LeftParen |(|
//@[62:63) RightParen |)|
//@[64:66) LogicalAnd |&&|
//@[67:70) Identifier |sys|
//@[70:71) Dot |.|
//@[71:86) Identifier |greaterOrEquals|
//@[86:87) LeftParen |(|
//@[87:88) RightParen |)|
//@[88:89) NewLine |\n|
  equals: sys.equals()
//@[02:08) Identifier |equals|
//@[08:09) Colon |:|
//@[10:13) Identifier |sys|
//@[13:14) Dot |.|
//@[14:20) Identifier |equals|
//@[20:21) LeftParen |(|
//@[21:22) RightParen |)|
//@[22:23) NewLine |\n|
  bool: sys.not() || sys.and() || sys.or()
//@[02:06) Identifier |bool|
//@[06:07) Colon |:|
//@[08:11) Identifier |sys|
//@[11:12) Dot |.|
//@[12:15) Identifier |not|
//@[15:16) LeftParen |(|
//@[16:17) RightParen |)|
//@[18:20) LogicalOr ||||
//@[21:24) Identifier |sys|
//@[24:25) Dot |.|
//@[25:28) Identifier |and|
//@[28:29) LeftParen |(|
//@[29:30) RightParen |)|
//@[31:33) LogicalOr ||||
//@[34:37) Identifier |sys|
//@[37:38) Dot |.|
//@[38:40) Identifier |or|
//@[40:41) LeftParen |(|
//@[41:42) RightParen |)|
//@[42:43) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// we can get function completions from namespaces
//@[50:51) NewLine |\n|
// #completionTest(22) -> azFunctions
//@[37:38) NewLine |\n|
var azFunctions = az.a
//@[00:03) Identifier |var|
//@[04:15) Identifier |azFunctions|
//@[16:17) Assignment |=|
//@[18:20) Identifier |az|
//@[20:21) Dot |.|
//@[21:22) Identifier |a|
//@[22:23) NewLine |\n|
// #completionTest(24) -> sysFunctions
//@[38:39) NewLine |\n|
var sysFunctions = sys.a
//@[00:03) Identifier |var|
//@[04:16) Identifier |sysFunctions|
//@[17:18) Assignment |=|
//@[19:22) Identifier |sys|
//@[22:23) Dot |.|
//@[23:24) Identifier |a|
//@[24:26) NewLine |\n\n|

// #completionTest(33) -> sysFunctions
//@[38:39) NewLine |\n|
var sysFunctionsInParens = (sys.a)
//@[00:03) Identifier |var|
//@[04:24) Identifier |sysFunctionsInParens|
//@[25:26) Assignment |=|
//@[27:28) LeftParen |(|
//@[28:31) Identifier |sys|
//@[31:32) Dot |.|
//@[32:33) Identifier |a|
//@[33:34) RightParen |)|
//@[34:36) NewLine |\n\n|

// missing method name
//@[22:23) NewLine |\n|
var missingMethodName = az.()
//@[00:03) Identifier |var|
//@[04:21) Identifier |missingMethodName|
//@[22:23) Assignment |=|
//@[24:26) Identifier |az|
//@[26:27) Dot |.|
//@[27:28) LeftParen |(|
//@[28:29) RightParen |)|
//@[29:31) NewLine |\n\n|

// missing indexer
//@[18:19) NewLine |\n|
var missingIndexerOnLiteralArray = [][][]
//@[00:03) Identifier |var|
//@[04:32) Identifier |missingIndexerOnLiteralArray|
//@[33:34) Assignment |=|
//@[35:36) LeftSquare |[|
//@[36:37) RightSquare |]|
//@[37:38) LeftSquare |[|
//@[38:39) RightSquare |]|
//@[39:40) LeftSquare |[|
//@[40:41) RightSquare |]|
//@[41:42) NewLine |\n|
var missingIndexerOnIdentifier = nonExistentIdentifier[][1][]
//@[00:03) Identifier |var|
//@[04:30) Identifier |missingIndexerOnIdentifier|
//@[31:32) Assignment |=|
//@[33:54) Identifier |nonExistentIdentifier|
//@[54:55) LeftSquare |[|
//@[55:56) RightSquare |]|
//@[56:57) LeftSquare |[|
//@[57:58) Integer |1|
//@[58:59) RightSquare |]|
//@[59:60) LeftSquare |[|
//@[60:61) RightSquare |]|
//@[61:63) NewLine |\n\n|

// empty parens - should produce expected expression diagnostic
//@[63:64) NewLine |\n|
var emptyParens = ()
//@[00:03) Identifier |var|
//@[04:15) Identifier |emptyParens|
//@[16:17) Assignment |=|
//@[18:19) LeftParen |(|
//@[19:20) RightParen |)|
//@[20:22) NewLine |\n\n|

// #completionTest(26) -> symbols
//@[33:34) NewLine |\n|
var anotherEmptyParens = ()
//@[00:03) Identifier |var|
//@[04:22) Identifier |anotherEmptyParens|
//@[23:24) Assignment |=|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:29) NewLine |\n\n|

// keywords can't be called like functions
//@[42:43) NewLine |\n|
var nullness = null()
//@[00:03) Identifier |var|
//@[04:12) Identifier |nullness|
//@[13:14) Assignment |=|
//@[15:19) NullKeyword |null|
//@[19:20) LeftParen |(|
//@[20:21) RightParen |)|
//@[21:22) NewLine |\n|
var truth = true()
//@[00:03) Identifier |var|
//@[04:09) Identifier |truth|
//@[10:11) Assignment |=|
//@[12:16) TrueKeyword |true|
//@[16:17) LeftParen |(|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
var falsehood = false()
//@[00:03) Identifier |var|
//@[04:13) Identifier |falsehood|
//@[14:15) Assignment |=|
//@[16:21) FalseKeyword |false|
//@[21:22) LeftParen |(|
//@[22:23) RightParen |)|
//@[23:25) NewLine |\n\n|

var partialObject = {
//@[00:03) Identifier |var|
//@[04:17) Identifier |partialObject|
//@[18:19) Assignment |=|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
  2: true
//@[02:03) Integer |2|
//@[03:04) Colon |:|
//@[05:09) TrueKeyword |true|
//@[09:10) NewLine |\n|
  +
//@[02:03) Plus |+|
//@[03:04) NewLine |\n|
  3 : concat('s')
//@[02:03) Integer |3|
//@[04:05) Colon |:|
//@[06:12) Identifier |concat|
//@[12:13) LeftParen |(|
//@[13:16) StringComplete |'s'|
//@[16:17) RightParen |)|
//@[17:18) NewLine |\n|
  
//@[02:03) NewLine |\n|
  's' 
//@[02:05) StringComplete |'s'|
//@[06:07) NewLine |\n|
  's' \
//@[02:05) StringComplete |'s'|
//@[06:07) Unrecognized |\|
//@[07:08) NewLine |\n|
  'e'   =
//@[02:05) StringComplete |'e'|
//@[08:09) Assignment |=|
//@[09:10) NewLine |\n|
  's' :
//@[02:05) StringComplete |'s'|
//@[06:07) Colon |:|
//@[07:09) NewLine |\n\n|

  a
//@[02:03) Identifier |a|
//@[03:04) NewLine |\n|
  b $
//@[02:03) Identifier |b|
//@[04:05) Unrecognized |$|
//@[05:06) NewLine |\n|
  a # 22
//@[02:03) Identifier |a|
//@[04:05) Unrecognized |#|
//@[06:08) Integer |22|
//@[08:09) NewLine |\n|
  c :
//@[02:03) Identifier |c|
//@[04:05) Colon |:|
//@[05:06) NewLine |\n|
  d  : %
//@[02:03) Identifier |d|
//@[05:06) Colon |:|
//@[07:08) Modulo |%|
//@[08:09) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// dangling decorators - to make sure the tests work, please do not add contents after this line
//@[96:97) NewLine |\n|
@concat()
//@[00:01) At |@|
//@[01:07) Identifier |concat|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
@sys.secure()
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:11) Identifier |secure|
//@[11:12) LeftParen |(|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
xxxxx
//@[00:05) Identifier |xxxxx|
//@[05:08) NewLine |\n\n\n|


@minLength()
//@[00:01) At |@|
//@[01:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:21) NewLine |\n\n\n\n\n\n\n\n\n|









//@[00:00) EndOfFile ||
