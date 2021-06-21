package cubyType;

public class CubyArrayType extends CubyBaseType{
    private CubyBaseType[] value;
    private int length;

    public CubyArrayType(){
        value = null;
    }

    public CubyArrayType(CubyBaseType[] array){
        value = array;
    }

    public CubyBaseType[] getValue() {
        return value;
    }

    public void setValue(CubyBaseType[] value) {
        this.value = value;
    }
}
