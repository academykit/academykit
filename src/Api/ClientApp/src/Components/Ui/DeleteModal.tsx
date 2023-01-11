import { Button, Group, Modal } from "@mantine/core";
import React from "react";

const DeleteModal = ({
  onClose,
  title,
  open,
  onConfirm,
}: {
  onClose: Function;
  title: string;
  open: boolean;
  onConfirm: Function;
}) => {
  return (
    <Modal title={title} opened={open} onClose={() => onClose()}>
      <Group mt={10}>
        <Button
          onClick={() => {
            onConfirm();
          }}
        >
          Confirm
        </Button>
        <Button variant="outline" onClick={() => onClose()}>
          Cancel
        </Button>
      </Group>
    </Modal>
  );
};

export default DeleteModal;
