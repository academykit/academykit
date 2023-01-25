import { useEffect, useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import { UseFormReturnType } from "@mantine/form/lib/types";
import FilePondPluginImageValidateSize from "filepond-plugin-image-validate-size";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import "filepond-plugin-image-preview/dist/filepond-plugin-image-preview.css";
import { FileAccess, uploadFile } from "@utils/services/fileService";
import { Text } from "@mantine/core";

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginImageValidateSize,
  FilePondPluginFileValidateType
);
type IProps = {
  formContext: () => UseFormReturnType<any, (values: any) => any>;
  label?: string;
  FormField?: string;
  currentThumbnail?: string;
  width?: string;
};

const ThumbnailEditor = ({
  formContext,
  label = "files",
  FormField = "thumbnail",
  currentThumbnail,
  width,
}: IProps) => {
  const form = formContext();
  const [files, setFiles] = useState<any>([]);
  useEffect(() => {
    if (currentThumbnail) {
      setFiles([
        {
          source: currentThumbnail,
          options: {
            type: "local",
          },
        },
      ]);
    }
  }, [currentThumbnail]);

  return (
    <div
      style={{
        maxWidth: width ? "" : 470,
        position: "relative",
        width: width ? width : "",
      }}
    >
      <FilePond
        instantUpload={true}
        acceptedFileTypes={[
          "image/png",
          "image/jpeg",
          "image/gif",
          "image/jpg",
        ]}
        // imageValidateSizeMinWidth={639}
        // imageValidateSizeMinHeight={359}
        // imageValidateSizeMinResolution={229401}
        files={files}
        labelIdle={`Drag & Drop your ${label} or <span class="filepond--label-action">Browse</span>`}
        onaddfile={(error, file) => {}}
        onupdatefiles={setFiles}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        server={{
          remove: () => {
            setFiles([]);
            form.setFieldValue(FormField, "");
          },
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
              const res = await uploadFile(file as File, FileAccess.Public);
              load(res.data);
              form.setFieldValue(FormField, res.data);
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
      {form.errors[FormField] && (
        <Text color={"red"} size={"xs"} pos="absolute" top={"100%"}>
          {form.errors[FormField]}
        </Text>
      )}
    </div>
  );
};

export default ThumbnailEditor;
