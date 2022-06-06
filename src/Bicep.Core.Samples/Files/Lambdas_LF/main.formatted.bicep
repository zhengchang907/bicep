var doggos = [
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]

var numbers = range(0, 4)

var sayHello = map(doggos, i=>'Hello ${i}!')

var isEven = filter(numbers, i=>0 == i % 2)

var evenDoggosNestedLambdas = map(filter(numbers, i=>contains(filter(numbers, j=>0 == j % 2), i)), x=>doggos[x])
