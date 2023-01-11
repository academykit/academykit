import React, { useCallback } from "react";
import { UseFormReturnType } from "@mantine/form";
import { EFIleUploadType } from "@utils/enums";
import { uploadFile } from "@utils/services/fileService";
import RichTextEditor from "@mantine/rte";

type IProps = {
  formContext: () => UseFormReturnType<any, (values: any) => any>;
  label?: string;
};
const TextEditor = ({ formContext, label }: IProps) => {
  const handleImageUpload = useCallback(
    (file: File): Promise<string> =>
      new Promise((resolve, reject) => {
        const formData = new FormData();
        formData.append("image", file);

        uploadFile(file, EFIleUploadType.image)
          .then((result) => resolve(result.data))
          .catch(() => reject(new Error("Upload failed")));
      }),
    []
  );

  const form = formContext();
  return (
    <RichTextEditor
      onImageUpload={handleImageUpload}
      style={{ wordBreak: "break-all" }}
      {...form.getInputProps(label ?? "description")}
    />
  );
};

export default TextEditor;
