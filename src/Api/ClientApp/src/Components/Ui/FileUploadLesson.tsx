import { useEffect, useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import { Box, Text } from "@mantine/core";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import FilePondPluginImageValidateSize from "filepond-plugin-image-validate-size";
import {
  FileAccess,
  uploadFile,
  uploadVideo,
} from "@utils/services/fileService";
import { UseFormReturnType } from "@mantine/form";

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType,
  FilePondPluginImageValidateSize
);

const FileUploadLesson = ({
  currentFile,
  formContext,
}: {
  currentFile?: string;
  formContext: () => UseFormReturnType<any, (values: any) => any>;
}) => {
  useEffect(() => {
    if (currentFile) {
      setFiles([
        {
          source: currentFile,
          options: {
            type: "local",
          },
        },
      ]);
    }
  }, [currentFile]);

  const [files, setFiles] = useState<any>([]);
  const form = formContext();
  return (
    <Box sx={{ maxWidth: 470 }} pos="relative">
      <FilePond
        instantUpload={true}
        files={files}
        labelIdle={`Drag & Drop your File or <span class="filepond--label-action">Browse</span>`}
        onaddfile={(error, file) => {}}
        onremovefile={() => form.setFieldValue("documentUrl", "")}
        onupdatefiles={setFiles}
        acceptedFileTypes={["application/pdf"]}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        server={{
          remove: null,
          revert: null,
          //processing a file
          process: async (
            fieldName,
            file,
            metadata,
            load,
            error,
            progress,
            abort
          ) => {
            try {
              const res = await uploadFile(file as File, FileAccess.Private);
              load(res.data);
              form.setFieldValue("documentUrl", res.data);
            } catch (e) {
              error("Unable to upload file");
            }
            return {
              abort: () => {
                abort();
              },
            };
          },
          load: async (source, load, error, progress, abort, headers) => {
            await fetch(
              `${source}?cache=${Math.random().toString(36).substring(2, 7)}`
            )
              .then(async (r) => {
                load(await r.blob());
              })
              .catch((r) => error(r));
            // Should expose an abort method so the request can be cancelled
            return {
              abort: () => {
                abort();
              },
            };
          },
        }}
      />
      {form.errors["documentUrl"] && (
        <Text color={"red"} size={"xs"} pos="absolute" top={"100%"}>
          {form.errors["documentUrl"]}
        </Text>
      )}
    </Box>
  );
};

export default FileUploadLesson;
