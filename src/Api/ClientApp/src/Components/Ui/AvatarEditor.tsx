import { UseFormReturnType } from "@mantine/form";
import { uploadFile } from "@utils/services/fileService";
import { useEffect, useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import FilePondPluginImageCrop from "filepond-plugin-image-crop";
import FilePondPluginImageTransform from "filepond-plugin-image-transform";
import "filepond-plugin-image-preview/dist/filepond-plugin-image-preview.css";
import {
  default as FilePondPluginFileValidateSize,
  default as FilePondPluginImageResize,
} from "filepond-plugin-file-validate-size";
import { EFIleUploadType } from "@utils/enums";
registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType,
  FilePondPluginFileValidateSize,
  FilePondPluginImageResize,
  FilePondPluginImageCrop,
  FilePondPluginImageTransform
);

type IProps = {
  formContext: () => UseFormReturnType<any, (values: any) => any>;
  label?: string;
  formField?: string;
  url?: string | null;
  onRemoveSuccess: Function;
  onUploadSuccess: Function;
};

const AvatarEditor = ({
  formContext,
  label = "files",
  formField = "imageUrl",
  url,
  onRemoveSuccess,
  onUploadSuccess,
}: IProps) => {
  const form = formContext();
  const [files, setFiles] = useState<any>([]);
  useEffect(() => {
    if (url) {
      setFiles([
        {
          source: url,
          options: {
            type: "local",
          },
        },
      ]);
    }
  }, [url]);
  const handleToRemoveImage = (errRes: any, file: any) => {
    setFiles([]);
    if (errRes == null) {
      onRemoveSuccess();
    }
  };
  return (
    <div style={{ maxWidth: 200 }}>
      <FilePond
        imagePreviewHeight={170}
        imageCropAspectRatio="1:1"
        styleLoadIndicatorPosition="center bottom"
        instantUpload={true}
        onremovefile={(errRes, file) => handleToRemoveImage(errRes, file)}
        acceptedFileTypes={["image/png", "image/jpeg", "image/gif"]}
        // imageValidateSizeMinWidth={639}
        // imageValidateSizeMinHeight={359}
        // imageValidateSizeMinResolution={229401}
        stylePanelLayout="compact circle"
        stylePanelAspectRatio="1:1"
        files={files}
        labelIdle={`Drag & Drop your ${label} or <span class="filepond--label-action">Browse</span>`}
        onupdatefiles={setFiles}
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
              const res = await uploadFile(file as File, EFIleUploadType.image);
              load(res.data);
              if (onUploadSuccess) {
                onUploadSuccess(res.data);
              }
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
    </div>
  );
};

export default AvatarEditor;
