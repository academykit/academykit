import { Check, Cross } from "@components/Icons";
import { Flex, Loader, TextInput, UnstyledButton } from "@mantine/core";
import React from "react";
import classes from "./styles/inlineInput.module.css";

type InlineInputProps = {
  disabled?: boolean;
  onCloseEdit: () => void;
  placeholder: string;
  isLoading?: boolean;
};

const InlineInput: React.FC<React.PropsWithChildren<InlineInputProps>> = ({
  onCloseEdit,
  placeholder,
  isLoading = false,
  ...props
}) => {
  return (
    <div style={{ display: "flex", alignItems: "center" }}>
      <TextInput autoFocus placeholder={placeholder} {...props} />
      {!isLoading ? (
        <>
          <UnstyledButton type="submit" className={classes.check}>
            <Check />
          </UnstyledButton>
          <div onClick={() => onCloseEdit()} className={classes.cross}>
            <Cross />
          </div>
        </>
      ) : (
        <Flex align={"center"} ml={10}>
          <Loader />
        </Flex>
      )}
    </div>
  );
};

export default InlineInput;
