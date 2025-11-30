function Interpreter()
{
    this.vars = {};
    this.functions = {};
}
const OPERATORS = "+-*/^%="
const BRACKETS = "()"

let isOperator = (o) => OPERATORS.includes(o)
let isBracket = (o) =>  BRACKETS.includes(o)

Interpreter.prototype.tokenize = function (program)
{
    if (program === "")
        return [];
    var regex = /\s*(=>|[-+*\/\%=\(\)]|[A-Za-z_][A-Za-z0-9_]*|[0-9]*\.?[0-9]+)\s*/g;
    return program.split(regex).filter(function (s) { return !s.match(/^\s*$/); });
};

Interpreter.prototype.input = function (expr)
{
    if(expr.trim() == '')
    return ''
    if(expr[0]=='f' && expr[1] == 'n'){
        this.input_function(expr)
        return ''
    }
    
    
    let tokens = this.tokenize(expr)//.reverse();
    if(!this.validate_input(tokens)){
        throw 'Invalid input'
    }
    let result = this.calc(tokens)
  return result
};

Interpreter.prototype.validate_input = function(tokens){
    if(tokens.length == 1) return true
    for(const token of tokens){
        if(isOperator(token) || isText(token)){
            return true
        }
    }
    return false
}
Interpreter.prototype.input_function = function(expr){
    let tokens = this.tokenize(expr)
    let func = {
        'args': [],
        'body' : []
    }
    let name = tokens[1]
    if(Object.keys(this.vars).includes(name)){
        throw 'variable ith name of this function exist'
    }
    let i = 2;
    for(i; i < tokens.length; i++){
        if(tokens[i] == '=>'){
            i++
            break
        }
if(func.args.includes(tokens[i]))
        throw 'Cant declare multiple same arguments for function'
        func.args.push(tokens[i])
        
    }
    for(i;i < tokens.length;i++){
        if(isText(tokens[i]) &&(!func.args.includes(tokens[i]))){
            throw `Unknown identifier ${tokens[i]}`
        }
        func.body.push(tokens[i])
        }
    func.body = func.body
    this.functions[name] = func
}

function isText(text){
    if(text.length > 1)
        return isNaN(text)
    let regex = /[a-zA-Z_]/
    return regex.test(text)

}
function isSpecialCharacter(text){
let regex = /[\(\)\+\-\*\/%\^]/
return regex.test(text)
}



class Stack{
    constructor(){
        this.arr = []
    }
    push(value){
        this.arr.push(value)
    }
    top(){
        return this.arr[this.arr.length - 1]
    }
    length(){
        return this.arr.length
    }
    pop(){
        return this.arr.pop()
    }
}

const left_bracket = '('
const right_bracket = ')'

Interpreter.prototype.turnToPostfix = function(input){
    let Postfix = []
    let Operators = new Stack()
    let Functions = new Stack()
    let numbers_in_row = 0
    for( elem of input){
  if(elem == 'fn')
        throw 'cant declare functions inside expression'

        if(isOperator(elem) || isBracket(elem) ){
            numbers_in_row = 0
            if (elem == right_bracket){

                while (Operators.top() != left_bracket){
                    Postfix.push(Operators.pop())
                    
                }
                Operators.pop()
                Functions.pop()
                if(Functions.top() && Functions.top().name){
                    Functions.top().await--
                }
                
                
            }
            else
            if(Operators.length() == 0 || Operators.top() == left_bracket){
                
                Operators.push(elem)
                if(elem == left_bracket){
                    Functions.push(left_bracket)
                    
                }
                
            }
            else
            if (elem == left_bracket)
            {
                Operators.push(elem)
                Functions.push(left_bracket)
            }
            else{
                if (get_operator_priority(elem) == get_operator_priority(Operators.top()) && elem != '='){

                    Postfix.push(Operators.pop())
                }
                else
                    while (Operators.length() != 0 && Operators.top() != left_bracket && get_operator_priority(elem) <= get_operator_priority(Operators.top()) && elem != '=' ){

                        Postfix.push(Operators.pop())
                        
                    }
                Operators.push(elem)
            }
        }
        else{
            if(this.functions[elem]){
                numbers_in_row = 0
                Functions.push({
                    'name':elem,
                    'await': this.functions[elem].args.length
                })
            }
            else{
                numbers_in_row++
                
                if (numbers_in_row > 1){
                    while (Operators.length() != 0){
        
                        Postfix.push(Operators.pop())
                    }
                }
            
                if(Functions.top() && Functions.top().await){
                    Functions.top().await--
                }
                Postfix.push(elem)       
        }
        }
        while(Functions.top() && Functions.top().await == 0){
            if(Functions.top().await == 0){
                Postfix.push(Functions.pop().name)
            }
            if(Functions.top() && Functions.top().name)
            Functions.top().await--
        }

    }
    
    
    while (Operators.length() != 0){
        
        Postfix.push(Operators.pop())
    }   
    while(Functions.length()){
        Postfix.push(Functions.pop().name)
    }
    return Postfix

}


Interpreter.prototype.calc = function(expr,scope = this){
    expr = this.turnToPostfix(expr)
    let S = new Stack()
    for(elem of expr){
        if(isOperator(elem)){
            b = this.get_variable_or_token(S.pop(),scope)
            
            a = S.pop()
            if(a == undefined){
                throw ' empty assignmet'
            }
            if(elem == '='){      
                if(a == 'standallone')
                throw ' cant assign to fucntion'
                if(isNaN(a)){

                    scope.vars[a] = b
                    S.push(b)
                }
                else{
                    S.push(b== a)
                }
            }
            else{
                if(a == 'standallone')
                a = S.pop()
            a = this.get_variable_or_token(a,scope)

            switch (elem) {
                case '-':
                    S.push(a-b)
                    break;
                case '+':
                    S.push(b+a)
                    break;
                case '*':
                    S.push(b*a)
                    break;
                case '/':
                    S.push(a/b)
                    break;
                case '%':
                    S.push(a%b)
                    break;
                case '^':
                    S.push(b**a)
                    break;
                
            }
        }
        }
        else
        {
            if(this.functions[elem]){ 
                let calling = elem                        
                //creating new scope for function
                
                let new_scope = {
                    'vars': {}
                }
                //filling function params with args 
                for(argument of this.functions[calling].args){
                    new_scope.vars[argument] = scope.get_variable_or_token(S.pop())
                       
                }
                S.push(this.calc(this.functions[calling].body,new_scope))
                if(this.functions[calling].args.length == 0){
                    S.push('standallone')
                    
                   
                }
            }
            else
            S.push(elem)
        }
            
    }
    if(S.length() != 1){
     if(S.top() == 'standallone')
      S.pop()
      else 
    throw 'something wrong'
    }
    return this.get_variable_or_token(S.top(),scope)
}
/** if token is number -> returns */
Interpreter.prototype.get_variable_or_token  = function(token,scope = this){

    if(isNaN(token)){
        token = scope.vars[token]
    if(token == undefined)
    throw `no variable ${token}`; 
    }

    return +token
}
function get_operator_priority(oper){
    return operator_priorities[oper] >> 0
}



let operator_priorities = {
    '-':1,
    '+':1,
    '*':2,
    '%':2,
    '^':3,
    '/':2,
    '=':0,

}
