import {
  Button,
  createStyles,
  Flex,
  Loader,
  TextInput,
  UnstyledButton,
} from "@mantine/core";
import React from "react";
import { Check, Cross } from "@components/Icons";
import { UseFormReturnType } from "@mantine/form";

type InlineInputProps = {
  disabled?: boolean;
  onCloseEdit: Function;
  placeholder: string;
  isLoading?: boolean;
};

const useStyles = createStyles((theme) => ({
  check: {
    cursor: "pointer",
    marginLeft: "10px",
    marginTop: "auto",
  },
  cross: {
    marginTop: "auto",
    cursor: "pointer",
    marginLeft: "10px",
    lineHeight: "1.15",
  },
}));

const InlineInput: React.FC<React.PropsWithChildren<InlineInputProps>> = ({
  onCloseEdit,
  placeholder,
  isLoading = false,
  ...props
}) => {
  const { theme, classes } = useStyles();
  return (
    <div style={{ display: "flex" }}>
      <TextInput placeholder={placeholder} {...props} />
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
