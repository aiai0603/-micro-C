package cubyType;

public class CubyFloatType extends CubyBaseType {
    private float value;


    public CubyFloatType(){
        this.value = 0;
    }

    public CubyFloatType(float value){
        this.value = value;
    }

    public float getValue() {
        return value;
    }

    public void setValue(float value) {
        this.value = value;
    }
}
