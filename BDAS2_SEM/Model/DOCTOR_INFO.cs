using System.ComponentModel;
using System.Runtime.CompilerServices;

public class DOCTOR_INFO : INotifyPropertyChanged
{
    private int _doctorId;
    private int _avatarId;
    private string _firstName;
    private string _surname;
    private long _phone;
    private string _department;
    private byte[] _avatar;

    public int DoctorId
    {
        get => _doctorId;
        set
        {
            if (_doctorId != value)
            {
                _doctorId = value;
                OnPropertyChanged();
            }
        }
    }

    public int AvatarId
    {
        get => _avatarId;
        set
        {
            if (_avatarId != value)
            {
                _avatarId = value;
                OnPropertyChanged();
            }
        }
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (_firstName != value)
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }
    }

    public string Surname
    {
        get => _surname;
        set
        {
            if (_surname != value)
            {
                _surname = value;
                OnPropertyChanged();
            }
        }
    }

    public long Phone
    {
        get => _phone;
        set
        {
            if (_phone != value)
            {
                _phone = value;
                OnPropertyChanged();
            }
        }
    }

    public string Department
    {
        get => _department;
        set
        {
            if (_department != value)
            {
                _department = value;
                OnPropertyChanged();
            }
        }
    }

    public byte[] Avatar
    {
        get => _avatar;
        set
        {
            if (_avatar != value)
            {
                _avatar = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}