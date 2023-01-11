import { useEffect, useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import { Box } from "@mantine/core";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import { uploadVideo } from "@utils/services/fileService";
import { EFIleUploadType, LessonFileType } from "@utils/enums";

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType
);

const FileUploadLesson = ({
  setUrl,
  currentFile,
}: {
  setUrl: Function;
  currentFile?: string;
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
  return (
    <Box my={10} sx={{ maxWidth: 470 }}>
      <FilePond
        instantUpload={true}
        files={files}
        labelIdle={`Drag & Drop your File or <span class="filepond--label-action">Browse</span>`}
        onaddfile={(error, file) => {}}
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
              const res = await uploadVideo(file as File, LessonFileType.File);
              load(res.data);
              setUrl(() => res.data);
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
    </Box>
  );
};

export default FileUploadLesson;
