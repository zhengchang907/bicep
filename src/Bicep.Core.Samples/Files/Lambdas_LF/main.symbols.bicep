var doggos = [
//@[004:010) Variable doggos. Type: array. Declaration start char: 0, length: 54
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]

var numbers = range(0, 4)
//@[004:011) Variable numbers. Type: int[]. Declaration start char: 0, length: 25

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[027:028) Local i. Type: any. Declaration start char: 27, length: 1
//@[004:012) Variable sayHello. Type: string[]. Declaration start char: 0, length: 46

var isEven = filter(numbers, i => 0 == i % 2)
//@[029:030) Local i. Type: int. Declaration start char: 29, length: 1
//@[004:010) Variable isEven. Type: int[]. Declaration start char: 0, length: 45

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[080:081) Local j. Type: int. Declaration start char: 80, length: 1
//@[050:051) Local i. Type: int. Declaration start char: 50, length: 1
//@[103:104) Local x. Type: int. Declaration start char: 103, length: 1
//@[004:027) Variable evenDoggosNestedLambdas. Type: any[]. Declaration start char: 0, length: 118

