import React, { useCallback } from "react";
import { UseFormReturnType } from "@mantine/form";
import { FileAccess, uploadFile } from "@utils/services/fileService";
import RichTextEditor from "@mantine/rte";

type IProps = {
  formContext: () => UseFormReturnType<any, (values: any) => any>;
  label?: string;
  placeholder: string;
};
const TextEditor = ({ formContext, label, placeholder }: IProps) => {
  const handleImageUpload = useCallback(
    (file: File): Promise<string> =>
      new Promise((resolve, reject) => {
        const formData = new FormData();
        formData.append("image", file);

        uploadFile(file, FileAccess.Public)
          .then((result) => resolve(result.data))
          .catch(() => reject(new Error("Upload failed")));
      }),
    []
  );

  const form = formContext();
  return (
    <RichTextEditor
      placeholder={placeholder}
      onImageUpload={handleImageUpload}
      style={{ wordBreak: "break-all", fontSize: 18 }}
      {...form.getInputProps(label ?? "description")}
    />
  );
};

export default TextEditor;
