var doggos = [
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]

var numbers = range(0, 4)

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[4:12) [no-unused-vars (Warning)] Variable "sayHello" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sayHello|

var isEven = filter(numbers, i => 0 == i % 2)
//@[4:10) [no-unused-vars (Warning)] Variable "isEven" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |isEven|

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[4:27) [no-unused-vars (Warning)] Variable "evenDoggosNestedLambdas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |evenDoggosNestedLambdas|

