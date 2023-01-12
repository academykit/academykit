import { useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import { Box } from "@mantine/core";
import { createFormContext } from "@mantine/form";
import { UseForm, UseFormReturnType } from "@mantine/form/lib/types";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType
);

const DcoumentUpload = ({ setUrl }: { setUrl: Function }) => {
  const [files, setFiles] = useState<any>([]);
  return (
    <Box my={10} sx={{ maxWidth: 470 }}>
      <FilePond
        files={files}
        onaddfile={(error, file) => {
          if (!error) {
            setUrl("hello");
          }
        }}
        acceptedFileTypes={["application/pdf"]}
        onupdatefiles={setFiles}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        // server="/api"
        name="files"
        labelIdle='Drag & Drop your Document or <span class="filepond--label-action">Browse</span>'
      />
    </Box>
  );
};

export default DcoumentUpload;