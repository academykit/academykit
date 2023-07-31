import { TextInput, TextInputProps } from '@mantine/core';
import { FC, useEffect, useRef, useState } from 'react';

interface ICustomTextFieldWithAutoFocus extends TextInputProps {
  isViewMode?: boolean;
}

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
        autoComplete={isFocused ? 'on' : 'off'}
        onClick={handleClick}
        onBlur={handleBlur}
      />
    </>
  );
};

export const DynamicAutoFocusTextField: FC<ICustomTextFieldWithAutoFocus> = (
  props
) => {
  const { isViewMode } = props;
  const [isFocused, setIsFocused] = useState(!isViewMode);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (!isViewMode && inputRef.current) {
      inputRef.current.focus();
    }
  }, [isViewMode]);

  const handleClick = () => {
    setIsFocused(true);
  };

  const handleBlur = () => {
    setIsFocused(false);
  };

  return (
    <>
      <TextInput
        {...props}
        styles={{ input: { border: isViewMode ? 'none' : '' } }}
        autoComplete={isFocused ? 'on' : 'off'}
        onClick={handleClick}
        onBlur={handleBlur}
        ref={inputRef}
      />
    </>
  );
};

export default CustomTextFieldWithAutoFocus;
