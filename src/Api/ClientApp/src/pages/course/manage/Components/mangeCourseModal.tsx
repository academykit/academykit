import { Modal } from "@mantine/core";
import React, { useState } from "react";

const ManageCourseModal = ({
  opened,
  setOpened,
}: {
  opened: boolean;
  setOpened: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  return (
    <Modal opened={opened} onClose={() => setOpened(false)}>
      Hello
    </Modal>
  );
};

export default ManageCourseModal;
