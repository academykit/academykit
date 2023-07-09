import { TextInput, TextInputProps } from "@mantine/core";
import React, { FC, useState } from "react";

interface ICustomTextFieldWithAutoFocus extends TextInputProps {}

const CustomTextFieldWithAutoFocus: FC<ICustomTextFieldWithAutoFocus> = (
  props
) => {
  const [isFocused, setIsFocused] = useState(false);

  const handleClick = () => {
    setIsFocused(true);
  };

  const handleBlur = () => {
    setIsFocused(false);
  };
  return (
    <TextInput
      {...props}
      autoComplete={isFocused ? "on" : "off"}
      autoFocus={isFocused}
      onClick={handleClick}
      onBlur={handleBlur}
    />
  );
};

export default CustomTextFieldWithAutoFocus;
