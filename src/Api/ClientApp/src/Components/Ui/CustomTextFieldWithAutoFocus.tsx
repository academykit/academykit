import { TextInput, TextInputProps } from "@mantine/core";
import  { FC, useEffect, useRef, useState } from "react";

interface ICustomTextFieldWithAutoFocus extends TextInputProps {}

const CustomTextFieldWithAutoFocus: FC<ICustomTextFieldWithAutoFocus> = (
  props
) => {
  const myRef = useRef<HTMLInputElement>(null);
  useEffect(() => {
    myRef?.current?.focus();
  }, [myRef.current]);

  // const mergedRef = useMergedRef(props.itemRef,)
  const [isFocused, setIsFocused] = useState(false);

  const handleClick = () => {
    setIsFocused(true);
  };

  const handleBlur = () => {
    setIsFocused(false);
  };
  return (
    <>
      <TextInput
        ref={myRef}
        {...props}
        autoComplete={isFocused ? "on" : "off"}
        autoFocus={isFocused}
        onClick={handleClick}
        onBlur={handleBlur}
      />
    </>
  );
};

export default CustomTextFieldWithAutoFocus;
