Int fact(Int i){
    if(i == 1){
        return 1;
    }else{
        return i * fact(i - 1);
    }
}
Int main(){
    Int n;
    n=4;
    print("%d", fact(n));
}