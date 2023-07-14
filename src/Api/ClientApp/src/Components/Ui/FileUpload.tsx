import { useState } from 'react';
import { FilePond, registerPlugin } from 'react-filepond';
import 'filepond/dist/filepond.min.css';
import FilePondPluginImageExifOrientation from 'filepond-plugin-image-exif-orientation';
import FilePondPluginImagePreview from 'filepond-plugin-image-preview';
import FilePondPluginImageValidateSize from 'filepond-plugin-image-validate-size';
import FilePondPluginFileValidateType from 'filepond-plugin-file-validate-type';
import 'filepond-plugin-image-preview/dist/filepond-plugin-image-preview.css';
import { useAddGroupAttachment } from '@utils/services/groupService';
import { showNotification } from '@mantine/notifications';
import errorType from '@utils/services/axiosError';
import { useTranslation } from 'react-i18next';
registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginImageValidateSize,
  FilePondPluginFileValidateType
);
type IProps = {
  label?: string;
  FormField?: string;
  currentThumbnail?: string;
  id: string;
  onSuccess: () => void;
  search: string;
};

const FileUpload = ({ label = 'files', onSuccess, id, search }: IProps) => {
  const [files, setFiles] = useState<any>([]);
  const postAttachment = useAddGroupAttachment(search);
  const { t } = useTranslation();

  const filePondProps = {
    labelInvalidField: t('Field contains invalid files'),
    labelFileWaitingForSize: t('Waiting for size'),
    labelFileSizeNotAvailable: t('Size not available'),
    labelFileLoading: t('Loading'),
    labelFileLoadError: t('Error during load'),
    labelFileProcessing: t('Processing'),
    labelFileProcessingComplete: t('Processing complete'),
    labelFileProcessingAborted: t('Processing aborted'),
    labelFileProcessingError: t('Error during processing'),
    labelFileProcessingRevertError: t('Error during revert'),
    labelFileRemoveError: t('Error during removal'),
    labelTapToCancel: t('Tap to cancel'),
    labelTapToRetry: t('Tap to retry'),
    labelTapToUndo: t('Tap to undo'),
    labelButtonRemoveItem: t('Remove'),
    labelButtonAbortItemLoad: t('Abort'),
    labelButtonRetryItemLoad: t('Retry'),
    labelButtonAbortItemProcessing: t('Abort'),
    labelButtonUndoItemProcessing: t('Undo'),
    labelButtonRetryItemProcessing: t('Retry'),
    labelButtonProcessItem: t('Process'),
  };

  return (
    <div style={{ maxWidth: 470, position: 'relative' }}>
      <FilePond
        instantUpload={true}
        files={files}
        labelIdle={`${t('drag_and_drop')} ${label} ${t(
          'or'
        )} <span class="filepond--label-action">${t('browse')}</span>`}
        onaddfile={() => {}}
        onupdatefiles={setFiles}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        server={{
          remove: null,
          revert: null,
          //processing a file
          process: async (_fieldName, file, _metadata, load, error) => {
            try {
              const res = await postAttachment.mutateAsync({
                groupIdentity: id as string,
                file: file as File,
              });
              load(res.data.url);
              showNotification({ message: t('add_attachment_success') });
              onSuccess();
            } catch (err) {
              const resError = errorType(err);
              error(resError);
              showNotification({
                message: resError,
                color: 'red',
              });
            }
          },
          load: async (source, load, error, _progress, abort) => {
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
        {...filePondProps}
      />
    </div>
  );
};

export default FileUpload;
