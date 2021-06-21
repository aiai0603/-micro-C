package cubyType;

public class CubyIntType extends CubyBaseType {
    private int value;

    public CubyIntType(){
        value = 0;
    }

    public CubyIntType(int value){
        this.value = value;
    }

    public int getValue() {
        return value;
    }

    public void setValue(int value) {
        this.value = value;
    }


}
