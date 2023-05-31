import { useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import { Box } from "@mantine/core";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import FilePondPluginImageValidateSize from "filepond-plugin-image-validate-size";
import { useTranslation } from "react-i18next";

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType,
  FilePondPluginImageValidateSize
);

const DcoumentUpload = ({ setUrl }: { setUrl: Function }) => {
  const [files, setFiles] = useState<any>([]);
  const { t } = useTranslation();
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
        labelIdle={`${t(
          "drag_document"
        )}<span class="filepond--label-action">${t("browse")}</span>`}
      />
    </Box>
  );
};

export default DcoumentUpload;
